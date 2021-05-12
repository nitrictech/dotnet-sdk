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
using ProtoClient = Nitric.Proto.Queue.v1.Queue.QueueClient;
using Nitric.Proto.Queue.v1;
using Nitric.Api.Common;
using System;

namespace Nitric.Api.Queue
{

    public class QueueClient : AbstractClient
    {
        protected ProtoClient client;
        public string Queue { get; private set; }
        private QueueClient(ProtoClient client, string queue)
        {
            this.Queue = queue;
            this.client = (client == null) ? new ProtoClient(this.GetChannel()) : client;
        }

        public void Send(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            var request = new QueueSendRequest
            {
                Queue = this.Queue,
                Task = EventToWire(task)
            };
            client.Send(request);
        }
        public PushResponse SendBatch(IList<Task> tasks)
        {
            RepeatedField<NitricTask> wireEvents = new RepeatedField<NitricTask>();
            foreach (Task task in tasks)
            {
                wireEvents.Add(EventToWire(task));
            }

            var request = new QueueSendBatchRequest { Queue = this.Queue };
            request.Tasks.AddRange(wireEvents);
            var response = client.SendBatch(request);
            List<FailedTask> failedTasks = new List<FailedTask>();
            foreach (Proto.Queue.v1.FailedTask fe in response.FailedTasks)
            {
                failedTasks.Add(WireToFailedEvent(fe));
            }
            return new PushResponse(failedTasks);
        }

        public List<Task> Receive(int depth)
        {
            if (depth < 1)
            {
                depth = 1;
            }
            var request = new QueueReceiveRequest { Queue = this.Queue, Depth = depth };
            var response = this.client.Receive(request);
            List<Task> items = new List<Task>();
            foreach (NitricTask nqi in response.Tasks)
            {
                items.Add(WireToQueueItem(nqi));
            }
            return items;
        }

        public void Complete(string leaseId)
        {
            if (string.IsNullOrEmpty(leaseId))
            {
                throw new ArgumentNullException("leaseId");
            }
            var request = new QueueCompleteRequest
            {
                Queue = this.Queue,
                LeaseId = leaseId
            };
            client.Complete(request);
        }
        private Task WireToQueueItem(NitricTask nitricTask)
        {
            return Task
                .NewBuilder()
                .RequestID(nitricTask.Id)
                .PayloadType(nitricTask.PayloadType)
                .Payload(nitricTask.Payload)
                .LeaseID(nitricTask.LeaseId)
                .Build();
        }

        private NitricTask EventToWire(Task task)
        {
            return new NitricTask
            {
                Id = task.ID,
                PayloadType = task.PayloadType,
                Payload = Util.ObjectToStruct(task.Payload)
            };
        }

        private FailedTask WireToFailedEvent(Proto.Queue.v1.FailedTask protoFailedEvent)
        {
            return new FailedTask.Builder()
                .RequestId(protoFailedEvent.Task.Id)
                .PayloadType(protoFailedEvent.Task.PayloadType)
                .Payload(protoFailedEvent.Task.Payload)
                .Message(protoFailedEvent.Message)
                .Build();
        }

        public static Builder NewBuilder() {
            return new Builder();
        }

        public class Builder
        {
            private string queue;
            private ProtoClient client;
            public Builder()
            {
                client = null;
                queue = "";
            }
            public Builder Client(ProtoClient client)
            {
                this.client = client;
                return this;
            }
            public Builder Queue(string queue)
            {
                this.queue = queue;
                return this;
            }
            public QueueClient Build()
            {
                return new QueueClient(client, queue);
            }
        }
    }
}