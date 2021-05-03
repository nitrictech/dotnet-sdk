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
namespace Nitric.Api.Common
{
    public class Event
    {
        public string RequestId { get; protected set; }
        public string PayloadType { get; protected set; }
        public Struct Payload { get; protected set; }
        private Event(string requestId, string payloadType, Struct payload)
        {
            Payload = payload;
            RequestId = requestId;
            PayloadType = payloadType;
        }
        public override string ToString()
        {
            return GetType().Name+
                    "[id=" + RequestId
                    + ", payloadType=" + PayloadType
                    + ", payload=" + Payload
                    + "]";
        }
        public class Builder
        {
            private string requestId;
            private string payloadType;
            private Struct payload;

            public Builder()
            {
                this.requestId = "";
                this.payloadType = "";
                this.payload = Util.ObjectToStruct(new Dictionary<string,string>());
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
            public Event Build()
            {
                return new Event(requestId, payloadType, payload);
            }
        }
    }
}
