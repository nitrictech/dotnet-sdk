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
﻿using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
namespace Nitric.Api.Queue
{
    public class FailedTask
    {
        public string Message { get; private set; }
        public Common.Event Event { get; private set; }
        private FailedTask(
            string requestId,
            string payloadType,
            Struct payload,
            string message)
        {
            this.Event = new Common.Event.Builder()
                .RequestId(requestId)
                .PayloadType(payloadType)
                .Payload(payload)
                .Build();
            this.Message = message;
        }
        public override string ToString()
        {
            return GetType().Name + "[event=" + Event + ", message=" + Message + "]";
        }

        public static Builder NewBuilder() {
            return new Builder();
        }

        public class Builder
        {
            private string requestId;
            private string payloadType;
            private Struct payload; 
            private string message;
            //Defines the defaults
            public Builder()
            {
                this.requestId = null;
                this.payloadType = null;
                this.payload = Common.Util.ObjectToStruct(new Dictionary<string, string>());
                this.message = null;
            }
            public Builder RequestId(string requestId)
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
            public Builder Message(string message)
            {
                this.message = message;
                return this;
            }
            public FailedTask Build()
            {
                return new FailedTask(requestId, payloadType, payload, message);
            }
        }
    }
}
