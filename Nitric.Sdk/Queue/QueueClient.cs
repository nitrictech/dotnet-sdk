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
using System.Collections.Generic;
using Google.Protobuf.Collections;
using GrpcClient = Nitric.Proto.Queue.v1.Queue.QueueClient;
using Nitric.Proto.Queue.v1;
using Nitric.Api.Common;
using System;

namespace Nitric.Api.Queue
{

    public class Queues : AbstractClient
    {
        internal GrpcClient Client { get; private set; }
        public Queues(GrpcClient client = null)
        {
            this.Client = (client == null) ? new GrpcClient(this.GetChannel()) : client;
        }
        public Queue Queue(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(queueName);
            }
            return new Queue(this, queueName);
        }
    }
    public class Queue
    {
        public string Name { get; private set; }
        internal Queues Queues;

        internal Queue(Queues queues, string name)
        {
            this.Name = name;
            this.Queues = queues;
        }
        public void Send(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            var request = new QueueSendRequest
            {
                Queue = this.Name,
                Task = EventToWire(task)
            };
            Queues.Client.Send(request);
        }
        public PushResponse SendBatch(IList<Task> tasks)
        {
            RepeatedField<NitricTask> wireEvents = new RepeatedField<NitricTask>();
            foreach (Task task in tasks)
            {
                wireEvents.Add(EventToWire(task));
            }

            var request = new QueueSendBatchRequest { Queue = this.Name };
            request.Tasks.AddRange(wireEvents);
            var response = Queues.Client.SendBatch(request);
            List<FailedTask> failedTasks = new List<FailedTask>();
            foreach (Proto.Queue.v1.FailedTask fe in response.FailedTasks)
            {
                failedTasks.Add(WireToFailedTask(fe));
            }
            return new PushResponse(failedTasks);
        }

        public List<Task> Receive(int depth)
        {
            if (depth < 1)
            {
                depth = 1;
            }
            var request = new QueueReceiveRequest { Queue = this.Name, Depth = depth };
            var response = this.Queues.Client.Receive(request);
            List<Task> items = new List<Task>();
            foreach (NitricTask nqi in response.Tasks)
            {
                items.Add(WireToQueueItem(nqi));
            }
            return items;
        }
        private Task WireToQueueItem(NitricTask nitricTask)
        {
            return Task
                .NewBuilder()
                .Id(nitricTask.Id)
                .PayloadType(nitricTask.PayloadType)
                .Payload(nitricTask.Payload)
                .LeaseID(nitricTask.LeaseId)
                .Queue(this)
                .Build();
        }

        private NitricTask EventToWire(Task task)
        {
            return new NitricTask
            {
                Id = task.ID,
                PayloadType = task.PayloadType,
                Payload = Util.ObjToStruct(task.Payload)
            };
        }

        private FailedTask WireToFailedTask(Proto.Queue.v1.FailedTask protoFailedEvent)
        {
            return FailedTask.NewBuilder()
                .Id(protoFailedEvent.Task.Id)
                .PayloadType(protoFailedEvent.Task.PayloadType)
                .Payload(protoFailedEvent.Task.Payload)
                .Message(protoFailedEvent.Message)
                .Build();
        }
    }
}