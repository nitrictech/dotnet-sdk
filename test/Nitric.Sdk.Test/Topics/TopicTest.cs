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
using Grpc.Core;
using Moq;
using GrpcClient = Nitric.Proto.Topics.v1.Topics.TopicsClient;
using Nitric.Proto.Topics.v1;
using Nitric.Sdk.Topics;
using Xunit;
using System.Threading.Tasks;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Test.Event
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
        public void TestBuildTopicWithName()
        {
            var topic = new Topic<TestProfile>("test-topic");
            Assert.NotNull(topic);
            Assert.Equal("test-topic", topic.Name);
        }

        [Fact]
        public void TestBuildTopicWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Topic<TestProfile>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Topic<TestProfile>(null)
            );
        }

        [Fact]
        public void TestTopicToString()
        {
            var topic = new Topic<TestProfile>("test-topic");

            Assert.Equal("Topic`1[name=test-topic]", topic.ToString());
        }

        [Fact]
        public async void TestPublish()
        {
            Mock<GrpcClient> ec = new Mock<GrpcClient>();

            ec.Setup(e => e.PublishAsync(It.IsAny<TopicPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<TopicPublishResponse>(Task.FromResult(new TopicPublishResponse()), null, null, null, null))
                .Verifiable();

            var topic = new Topic<TestProfile>("test-topic", ec.Object);

            var profile = new TestProfile
            { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            await topic.Publish(profile);

            ec.Verify(t => t.PublishAsync(It.IsAny<TopicPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestPublishToNonExistentTopic()
        {
            Mock<GrpcClient> ec = new Mock<GrpcClient>();

            ec.Setup(e => e.PublishAsync(It.IsAny<TopicPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified topic does not exist")))
                .Verifiable();

            var topic = new Topic<TestProfile>("test-topic", ec.Object);

            var profile = new TestProfile
            { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };

            try
            {
                await topic.Publish(profile);
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified topic does not exist\")", ne.Message);
            }

            ec.Verify(t => t.PublishAsync(It.IsAny<TopicPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    };
}
