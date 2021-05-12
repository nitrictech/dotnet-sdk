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
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using Nitric.Api.Common;
namespace Nitric.Api.Queue
{
    public class Task
    {
        public String ID { get; private set; }
        public String PayloadType { get; private set; }
        public Object Payload { get; private set; }
        public string LeaseID { get; private set; }
        private Task(string Id,
                          string payloadType,
                          Object payload,
                          string leaseID)
        {
            this.ID = Id;
            this.Payload = payload;
            this.PayloadType = payloadType;
            this.LeaseID = leaseID;
        }
        public override string ToString()
        {
            return GetType().Name + "[ID=" + this.ID + ", leaseId=" + this.LeaseID + "]";
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
            public Builder()
            {
                this.id = null;
                this.payloadType = null;
                this.payload = Common.Util.ObjectToStruct(new Dictionary<string, string>());
                this.leaseID = null;
            }
            public Builder RequestID(string id)
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
            public Task Build()
            {

                return new Task(id, payloadType, payload, leaseID);
            }
        }
    }
}
