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
using Xunit;


namespace Nitric.Sdk.Test.Api.Event
{
    public class EventTest
    {
        [Fact]
        public void TestBuild()
        {
            var payload = new TestProfile
                { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            var eventTest = new global::Nitric.Sdk.Event.Event<TestProfile>
                { Id = "id", Payload = payload, PayloadType = "payloadType" };

            Assert.NotNull(eventTest);
            Assert.Equal("id", eventTest.Id);
            Assert.Equal("payloadType", eventTest.PayloadType);
            Assert.Equal(payload, eventTest.Payload);
        }

        [Fact]
        public void TestToString()
        {
            var payload = new TestProfile
                { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            var eventTest = new global::Nitric.Sdk.Event.Event<TestProfile>
                { Id = "id", Payload = payload, PayloadType = "payloadType" };

            Assert.Equal("Event[id=id, payloadType=payloadType, payload={\"Name\":\"John Smith\",\"Age\":30.0,\"Addresses\":[\"123 street st\"]}]",
                eventTest.ToString());
        }
    }
}
