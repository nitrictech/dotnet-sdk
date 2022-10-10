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
namespace Nitric.Sdk.Queue
{
    public class FailedTask
    {
        public string Message { get; private set; }
        public Task Task { get; private set; }
        private FailedTask(
            string Id,
            string payloadType,
            Struct payload,
            string message)
        {
            this.Task = Task.NewBuilder()
                .Id(Id)
                .PayloadType(payloadType)
                .Payload(payload)
                .Build();
            this.Message = message;
        }
        public override string ToString()
        {
            return GetType().Name + "[task=" + Task + ", message=" + Message + "]";
        }

        public static Builder NewBuilder() {
            return new Builder();
        }

        public class Builder
        {
            private string ID;
            private string payloadType;
            private Struct payload;
            private string message;
            //Defines the defaults
            public Builder()
            {
                this.ID = null;
                this.payloadType = null;
                this.payload = Common.Util.ObjToStruct(new Dictionary<string, string>());
                this.message = null;
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
                return new FailedTask(ID, payloadType, payload, message);
            }
        }
    }
}
