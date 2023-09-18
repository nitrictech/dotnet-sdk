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
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using Nitric.Proto.Event.v1;
using Nitric.Sdk.Event;
using Xunit;

namespace Nitric.Sdk.Test.Api.Event
{
    public class TestProfile
    {
        public string Name;
        public double Age;
        public List<string> Addresses;
    }

    public class EventClientTest
    {
        [Fact]
        public void TestBuildEvents()
        {
            var evt = new EventsClient<TestProfile>();
            Assert.NotNull(evt);
        }
        [Fact]
        public void TestBuildTopicWithName()
        {
            var topic = new EventsClient<TestProfile>().Topic("test-topic");
            Assert.NotNull(topic);
            Assert.Equal("test-topic", topic.Name);
        }

        [Fact]
        public void TestBuildTopicWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new EventsClient<TestProfile>().Topic("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new EventsClient<TestProfile>().Topic(null)
            );
        }

        [Fact]
        public void TestPublish()
        {
            Mock<EventService.EventServiceClient> ec = new Mock<EventService.EventServiceClient>();

            ec.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new EventPublishResponse { Id = "1234" })
                .Verifiable();

            var topic = new EventsClient<TestProfile>(ec.Object).Topic("test-topic");

            var payload = new TestProfile
                { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            var evtToSend = new Sdk.Event.Event<TestProfile> { Id = "1234", Payload = payload, PayloadType = "payloadType"};

            var response = topic.Publish(evtToSend);

            Assert.Equal("1234", response);

            ec.Verify(t => t.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestPublishOnlyPayload()
        {
            Mock<EventService.EventServiceClient> ec = new Mock<EventService.EventServiceClient>();

            ec.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new EventPublishResponse { Id = "1234" })
                .Verifiable();

            var topic = new EventsClient<TestProfile>(ec.Object).Topic("test-topic");

            var payload = new TestProfile
                { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            var response = topic.Publish(payload);

            Assert.Equal("1234", response);

            ec.Verify(t => t.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestPublishToNonExistentTopic()
        {
            Mock<EventService.EventServiceClient> ec = new Mock<EventService.EventServiceClient>();

            ec.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified topic does not exist")))
                .Verifiable();

            var topic = new EventsClient<TestProfile>(ec.Object).Topic("test-topic");

            var payload = new TestProfile
                { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            var evtToSend = new Sdk.Event.Event<TestProfile> { Id = "1", Payload = payload, PayloadType = "payloadType"};

            try
            {
                var response = topic.Publish(evtToSend);
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified topic does not exist\")", ne.Message);
            }

            ec.Verify(t => t.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    };
}
