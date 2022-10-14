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
using Xunit;
using Nitric.Sdk.Event;
using EventModel = Nitric.Sdk.Event.Event;
using System.Collections.Generic;
using Nitric.Proto.Event.v1;
using Moq;
using Util = Nitric.Sdk.Common.Util;
using Grpc.Core;
using static Nitric.Sdk.Common.Util.Utils;

namespace Nitric.Test.Api.EventClient
{
    public class EventClientTest
    {
        [Fact]
        public void TestBuildEvents()
        {
            var evt = new Events();
            Assert.NotNull(evt);
        }
        [Fact]
        public void TestBuildTopicWithName()
        {
            var topic = new Events().Topic("test-topic");
            Assert.NotNull(topic);
            Assert.Equal("test-topic", topic.Name);
        }
        [Fact]
        public void TestBuildTopicWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Events().Topic("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Events().Topic(null)
            );
        }
        [Fact]
        public void TestPublish()
        {
            var payloadStruct = ObjToStruct(new Dictionary<string,string>());
            var evt = new NitricEvent { Id = "1", PayloadType = "payloadType", Payload = payloadStruct };
            var request = new EventPublishRequest { Topic = "test-topic", Event = evt };

            Mock<EventService.EventServiceClient> ec = new Mock<EventService.EventServiceClient>();

            ec.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new EventPublishResponse())
                .Verifiable();

            var topic = new Events(ec.Object).Topic("test-topic");

            var evtToSend = EventModel
                .NewBuilder()
                .Id("1")
                .PayloadType("payloadType")
                .Payload(new Dictionary<string, string>())
                .Build();

            var response = topic.Publish(evtToSend);

            ec.Verify(t => t.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [Fact]
        public void TestPublishToNonExistentTopic()
        {
            Mock<EventService.EventServiceClient> ec = new Mock<EventService.EventServiceClient>();

            ec.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified topic does not exist")))
                .Verifiable();

            var topic = new Events(ec.Object).Topic("test-topic");

            var evtToSend = EventModel
                .NewBuilder()
                .Id("1")
                .PayloadType("payloadType")
                .Payload(new Dictionary<string, string>())
                .Build();

            try
            {
                var response = topic.Publish(evtToSend);
            }
            catch (Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified topic does not exist\")", ne.Message);
            }

            ec.Verify(t => t.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    };
}
