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
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
namespace Nitric.Api.Queue
{
    public class Task
    {
        public Common.Event Event { get; private set; }
        public string LeaseID { get; private set; }
        private Task(string requestId,
                          string payloadType,
                          Struct payload,
                          string leaseID)
        {
            Event = new Common.Event.Builder()
                .RequestId(requestId)
                .PayloadType(payloadType)
                .Payload(payload)
                .Build();
            LeaseID = leaseID;
        }
        public override string ToString()
        {
            return GetType().Name + "[event=" + Event + ", leaseId=" + LeaseID + "]";
        }

        public static Builder NewBuilder() {
            return new Builder();
        }

        public class Builder
        {
            private string requestId;
            private string payloadType;
            private Struct payload;
            private string leaseID;
            public Builder()
            {
                this.requestId = null;
                this.payloadType = null;
                this.payload = Common.Util.ObjectToStruct(new Dictionary<string, string>());
                this.leaseID = null;
            }
            public Builder RequestID(string requestId)
            {
                this.requestId = requestId;
                return this;
            }
            public Builder PayloadType(string payloadType)
            {
                this.payloadType = payloadType;
                return this;
            }
            public Builder Payload(Struct payload)
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
                return new Task(requestId, payloadType, payload, leaseID);
            }
        }
    }
}
