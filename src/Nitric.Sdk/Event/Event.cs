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

namespace Nitric.Sdk.Event
{
    public class Event
    {
        public string Id { get; protected set; }
        public string PayloadType { get; protected set; }
        public Object Payload { get; protected set; }
        private Event(string Id, string payloadType, Object payload)
        {
            this.Payload = payload;
            this.Id = Id;
            this.PayloadType = payloadType;
        }
        public override string ToString()
        {
            return GetType().Name +
                    "[id=" + Id
                    + ", payloadType=" + PayloadType
                    + ", payload=" + Payload
                    + "]";
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private string ID;
            private string payloadType;
            private Object payload;

            public Builder()
            {
                this.ID = "";
                this.payloadType = "";
                this.payload = null;
            }
            public Builder Id(string Id)
            {
                this.ID = Id;
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
            public Event Build()
            {
                return new Event(ID, payloadType, payload);
            }
        }
    }
}
