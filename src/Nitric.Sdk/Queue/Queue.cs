// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Nitric.Proto.Queue.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Queue
{
    /// <summary>
    /// A reference to a queue in the queues service.
    /// </summary>
    public class Queue<T>
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string Name { get; internal set; }

        internal readonly QueuesClient QueuesClient;

        internal Queue(QueuesClient client, string name)
        {
            this.Name = name;
            this.QueuesClient = client;
        }

        /// <summary>
        /// Send a task to this queue.
        /// </summary>
        /// <param name="task">The task to send.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public void Send(Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var request = new QueueSendRequest
            {
                Queue = this.Name,
                Task = task.ToWire()
            };
            try
            {
                QueuesClient.Client.Send(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Send a task to this queue.
        /// </summary>
        /// <param name="payload">The payload of the task to send.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public void Send(T payload)
        {
            var task = new Task<T> { Payload = payload };

            var request = new QueueSendRequest
            {
                Queue = this.Name,
                Task = task.ToWire()
            };
            try
            {
                QueuesClient.Client.Send(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Send multiple tasks to this queue.
        /// </summary>
        /// <param name="tasks">The tasks to send.</param>
        /// <returns>Results of sending tasks to the queue, including any tasks that failed to be sent.</returns>
        /// <exception cref="NitricException"></exception>
        public List<FailedTask<T>> Send(params Task<T>[] tasks)
        {
            var wireEvents = new RepeatedField<NitricTask>();
            foreach (var task in tasks)
            {
                wireEvents.Add(task.ToWire());
            }

            var request = new QueueSendBatchRequest { Queue = this.Name };
            request.Tasks.AddRange(wireEvents);
            try
            {
                var response = QueuesClient.Client.SendBatch(request);
                var failedTasks = response.FailedTasks.Select(WireToFailedTask).ToList();
                return failedTasks;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Send multiple tasks to this queue.
        /// </summary>
        /// <param name="tasks">The tasks to send.</param>
        /// <returns>Results of sending tasks to the queue, including any tasks that failed to be sent.</returns>
        /// <exception cref="NitricException"></exception>
        public List<FailedTask<T>> Send(params T[] payloads)
        {
            var wireEvents = new RepeatedField<NitricTask>();
            foreach (var payload in payloads)
            {
                var task = new Task<T> { Payload = payload };
                wireEvents.Add(task.ToWire());
            }

            var request = new QueueSendBatchRequest { Queue = this.Name };
            request.Tasks.AddRange(wireEvents);
            try
            {
                var response = QueuesClient.Client.SendBatch(request);
                var failedTasks = response.FailedTasks.Select(WireToFailedTask).ToList();
                return failedTasks;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Request tasks from the queue to process.
        ///
        /// The number of tasks returned will be the same or less than the requested depth, based on the number of tasks
        /// available on the queue.
        /// </summary>
        /// <param name="depth">The maximum number of tasks to receive.</param>
        /// <returns>Tasks received from the queue.</returns>
        /// <exception cref="NitricException"></exception>
        public List<ReceivedTask<T>> Receive(int depth = 1)
        {
            if (depth < 1)
            {
                depth = 1;
            }

            var request = new QueueReceiveRequest { Queue = this.Name, Depth = depth };
            try
            {
                var response = this.QueuesClient.Client.Receive(request);
                return response.Tasks.Select(WireToQueueItem).ToList();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        private ReceivedTask<T> WireToQueueItem(NitricTask nitricTask)
        {
            var protoPayload = JsonConvert.SerializeObject(nitricTask.Payload);

            return new ReceivedTask<T>
            {
                Id = nitricTask.Id,
                PayloadType = nitricTask.PayloadType,
                Payload = JsonConvert.DeserializeObject<T>(protoPayload),
                LeaseId = nitricTask.LeaseId,
                Queue = this,
            };
        }

        private static FailedTask<T> WireToFailedTask(Proto.Queue.v1.FailedTask protoFailedEvent)
        {
            var protoPayload = JsonConvert.SerializeObject(protoFailedEvent.Task.Payload);

            return new FailedTask<T>
            {
                Message = protoFailedEvent.Message,
                Task = new Task<T>
                {
                    Id = protoFailedEvent.Task.Id,
                    PayloadType = protoFailedEvent.Task.PayloadType,
                    Payload = JsonConvert.DeserializeObject<T>(protoPayload),
                }
            };
        }
    }
}
