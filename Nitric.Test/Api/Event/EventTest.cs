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
﻿using System;
using System.Collections.Generic;
using Xunit;

namespace Nitric.Test.Api.EventClient
{
    public class EventTest
    {
        [Fact]
        public void TestBuild()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("name", "value");

            var eventTest = Nitric.Api.Event.Event.NewBuilder()
                .Id("id")
                .PayloadType("payloadType")
                .Payload(Nitric.Api.Common.Util.ObjToStruct(payload))
                .Build();

            Assert.NotNull(eventTest);
            Assert.Equal("id", eventTest.Id);
            Assert.Equal("payloadType", eventTest.PayloadType);
            Assert.Equal(Nitric.Api.Common.Util.ObjToStruct(payload), eventTest.Payload);
        }
        [Fact]
        public void TestToString()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("name", "value");

            var eventTest = Nitric.Api.Event.Event.NewBuilder()
                .Id("id")
                .PayloadType("payloadType")
                .Payload(Nitric.Api.Common.Util.ObjToStruct(payload))
                .Build();
            Assert.Equal("Event[id=id, payloadType=payloadType, payload={ \"name\": \"value\" }]", eventTest.ToString());
        }
    }
}
