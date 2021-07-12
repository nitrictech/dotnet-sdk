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
using Nitric.Proto.Queue.v1;

namespace Nitric.Api.Queue
{
    public class Task
    {
        public string ID { get; private set; }
        public string PayloadType { get; private set; }
        public object Payload { get; private set; }
        public string LeaseID { get; private set; }
        public Queue Queue { get; private set; }

        private Task(string Id,
                     string payloadType,
                     object payload,
                     string leaseID,
                     Queue queue)
        {
            this.ID = Id;
            this.Payload = payload;
            this.PayloadType = payloadType;
            this.LeaseID = leaseID;
            this.Queue = queue;
        }
        public override string ToString()
        {
            return GetType().Name + "[ID=" + this.ID + ", leaseId=" + this.LeaseID + "]";
        }

        public void Complete()
        {
            if (string.IsNullOrEmpty(this.LeaseID))
            {
                throw new ArgumentNullException("leaseId");
            }
            var request = new QueueCompleteRequest
            {
                Queue = this.Queue.Name,
                LeaseId = this.LeaseID,
            };
            Queue.Queues.Client.Complete(request);
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private string id;
            private string payloadType;
            private Object payload;
            private string leaseID;
            private Queue queue;
            public Builder()
            {
                this.id = null;
                this.payloadType = null;
                this.payload = Common.Util.ObjToStruct(new Dictionary<string, string>());
                this.leaseID = null;
                this.queue = null;
            }
            public Builder Id(string id)
            {
                this.id = id;
                return this;
            }
            public Builder PayloadType(string payloadType)
            {
                this.payloadType = payloadType;
                return this;
            }
            public Builder Payload(Object payload)
            {
                this.payload = payload;
                return this;
            }
            public Builder LeaseID(string leaseID)
            {
                this.leaseID = leaseID;
                return this;
            }
            public Builder Queue(Queue queue)
            {
                this.queue = queue;
                return this;
            }
            public Task Build()
            {
                return new Task(id, payloadType, payload, leaseID, queue);
            }
        }
    }
}
