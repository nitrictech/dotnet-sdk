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
        public string ID { get; protected set; }
        public string PayloadType { get; protected set; }
        public object Payload { get; protected set; }

        //Default constructor for derived class
        protected Task() { }

        private Task(string Id,
                     string payloadType,
                     object payload)
        {
            this.ID = Id;
            this.Payload = payload;
            this.PayloadType = payloadType;
        }
        public override string ToString()
        {
            return GetType().Name + "[ID=" + this.ID + "]";
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private string id;
            private string payloadType;
            private object payload;
            public Builder()
            {
                this.id = "";
                this.payloadType = "";
                this.payload = Common.Util.ObjToStruct(new Dictionary<string, string>());
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
            public Builder Payload(object payload)
            {
                this.payload = payload;
                return this;
            }
            public Task Build()
            {
                return new Task(id, payloadType, payload);
            }
        }
    }
    public class ReceivedTask : Task
    {
        public Queue Queue { get; private set; }
        public string LeaseID { get; private set; }

        private ReceivedTask(string id,
                             string payloadType,
                             object payload,
                             string leaseId,
                             Queue queue)
        {
            this.ID = id;
            this.PayloadType = payloadType;
            this.Payload = payload;
            this.LeaseID = leaseId;
            this.Queue = queue;
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
            try
            {
                Queue.Queues.Client.Complete(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.FromRpcException(re);
            }
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }
        public class Builder
        {
            private string id;
            private string payloadType;
            private object payload;
            private string leaseId;
            private Queue queue;

            public Builder()
            {
                this.id = "";
                this.payloadType = "";
                this.payload = null;
                this.leaseId = "";
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
            public Builder Payload(object payload)
            {
                this.payload = payload;
                return this;
            }
            public Builder LeaseId(string leaseId)
            {
                this.leaseId = leaseId;
                return this;
            }
            public Builder Queue(Queue queue)
            {
                this.queue = queue;
                return this;
            }
            public ReceivedTask Build()
            {
                return new ReceivedTask(this.id,
                                        this.payloadType,
                                        this.payload,
                                        this.leaseId,
                                        this.queue);
            }
        }
    }
}
