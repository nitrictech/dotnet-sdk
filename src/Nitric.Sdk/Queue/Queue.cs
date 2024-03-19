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
using Nitric.Sdk.Common;
using Nitric.Proto.Queues.v1;
using System.Threading.Tasks;

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

        internal readonly QueuesClient Queues;

        internal Queue(QueuesClient client, string name)
        {
            this.Name = name;
            this.Queues = client;
        }

        /// <summary>
        /// Send a task to this queue.
        /// </summary>
        /// <param name="tasks">The tasks to push to the queue.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public List<FailedMessage<T>> Enqueue(T task, params T[] tasks)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            var taskList = new List<T>() { task };

            taskList.AddRange(tasks);

            var request = new QueueEnqueueRequest
            {
                QueueName = Name,
            };

            var messages = taskList.Select(task => new QueueMessage
            {
                StructPayload = Struct.FromJsonSerializable(task)
            });

            request.Messages.AddRange(messages);

            try
            {
                var response = Queues.Client.Enqueue(request);

                return response.FailedMessages.Select(failedMessage => new FailedMessage<T>
                {
                    Details = failedMessage.Details,
                    Message = Struct.ToJsonSerializable<T>(failedMessage.Message.StructPayload),
                }).ToList();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Send a task to this queue.
        /// </summary>
        /// <param name="tasks">The tasks to push to the queue.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public async Task<List<FailedMessage<T>>> EnqueueAsync(T task, params T[] tasks)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            var taskList = new List<T>() { task };

            taskList.AddRange(tasks);

            var request = new QueueEnqueueRequest
            {
                QueueName = Name,
            };

            var messages = taskList.Select(task => new QueueMessage
            {
                StructPayload = Struct.FromJsonSerializable(task)
            });

            request.Messages.AddRange(messages);

            try
            {
                var response = await Queues.Client.EnqueueAsync(request);

                return response.FailedMessages.Select(failedMessage => new FailedMessage<T>
                {
                    Details = failedMessage.Details,
                    Message = Struct.ToJsonSerializable<T>(failedMessage.Message.StructPayload),
                }).ToList();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Dequeue tasks from the queue to process.
        ///
        /// The number of tasks returned will be the same or less than the requested depth, based on the number of tasks
        /// available on the queue.
        /// </summary>
        /// <param name="depth">The maximum number of tasks to dequeue.</param>
        /// <returns>Tasks dequeued from the queue.</returns>
        /// <exception cref="NitricException"></exception>
        public List<ReceivedMessage<T>> Dequeue(int depth = 1)
        {
            var request = new QueueDequeueRequest
            {
                QueueName = this.Name,
                Depth = Math.Max(depth, 1)
            };

            try
            {
                var response = this.Queues.Client.Dequeue(request);

                return response.Messages.Select(message => new ReceivedMessage<T>
                {
                    Queue = this,
                    LeaseId = message.LeaseId,
                    Message = Struct.ToJsonSerializable<T>(message.Message.StructPayload)
                }).ToList();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Dequeue tasks from the queue to process.
        ///
        /// The number of tasks returned will be the same or less than the requested depth, based on the number of tasks
        /// available on the queue.
        /// </summary>
        /// <param name="depth">The maximum number of tasks to dequeue.</param>
        /// <returns>Tasks dequeued from the queue.</returns>
        /// <exception cref="NitricException"></exception>
        public async Task<List<ReceivedMessage<T>>> DequeueAsync(int depth = 1)
        {
            var request = new QueueDequeueRequest
            {
                QueueName = this.Name,
                Depth = Math.Max(depth, 1)
            };

            try
            {
                var response = await this.Queues.Client.DequeueAsync(request);

                return response.Messages.Select(message => new ReceivedMessage<T>
                {
                    Queue = this,
                    LeaseId = message.LeaseId,
                    Message = Struct.ToJsonSerializable<T>(message.Message.StructPayload)
                }).ToList();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}


