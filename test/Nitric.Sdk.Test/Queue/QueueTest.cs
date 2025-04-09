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
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using Nitric.Proto.Queues.v1;
using GrpcClient = Nitric.Proto.Queues.v1.Queues.QueuesClient;
using Xunit;
using System.Threading.Tasks;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Test.Queue
{
    public class TestProfile
    {
        public string Name;
        public double Age;
        public List<string> Addresses;
    }

    public class QueueClientTest
    {
        [Fact]
        public void TestBuildQueueWithName()
        {
            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue");
            Assert.NotNull(queue);
            Assert.Equal("test-queue", queue.Name);
        }

        [Fact]
        public void TestBuildQueueWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Queue.Queue<TestProfile>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Queue.Queue<TestProfile>(null)
            );
        }

        [Fact]
        public void TestQueueToString()
        {
            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue");
            Assert.Equal("Queue`1[name=test-queue]", queue.ToString());
        }

        [Fact]
        public async void TestEnqueue()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e =>
                    e.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueEnqueueResponse>(Task.FromResult(new QueueEnqueueResponse()), null, null, null, null))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            await queue.Enqueue(new TestProfile { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } });

            qc.Verify(
                t => t.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestEnqueueNullMessage()
        {
            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue");

            Assert.ThrowsAsync<ArgumentNullException>(async () => await queue.Enqueue(null));
        }

        [Fact]
        public async void TestEnqueueMultipleMessagesWithFailedMessages()
        {
            FailedEnqueueMessage failedMessage = new FailedEnqueueMessage();
            failedMessage.Details = "I am a failed message... I failed my message";
            failedMessage.Message = new QueueMessage();

            List<FailedEnqueueMessage> failedMessages = new List<FailedEnqueueMessage>
            {
                failedMessage,
            };

            var queueBatchResponse = new QueueEnqueueResponse();
            queueBatchResponse.FailedMessages.AddRange(failedMessages);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueEnqueueResponse>(Task.FromResult(queueBatchResponse), null, null, null, null))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            var failedMessagesResp = await queue.Enqueue(new TestProfile { }, new TestProfile { });

            Assert.Equal("I am a failed message... I failed my message", failedMessagesResp[0].Details);

            qc.Verify(
                t => t.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestEnqueueMultipleMessagesWithNoFailedMessages()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueEnqueueResponse>(Task.FromResult(new QueueEnqueueResponse()), null, null, null, null))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            var failedMessages = await queue.Enqueue(new TestProfile { }, new TestProfile { });

            Assert.Empty(failedMessages);

            qc.Verify(
                t => t.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestEnqueueToNonExistentQueue()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e =>
                    e.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified queue does not exist")))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            try
            {
                await queue.Enqueue(new TestProfile());
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified queue does not exist\")",
                    ne.Message);
            }

            qc.Verify(
                t => t.EnqueueAsync(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void TestDequeueMessages()
        {
            var payload = new Google.Protobuf.WellKnownTypes.Struct();
            payload.Fields.Add("Name", Value.ForString("John Smith"));
            payload.Fields.Add("Age", Value.ForNumber(30.0));
            payload.Fields.Add("Addresses", Value.ForList(new[] { Value.ForString("123 street st") }));

            var messages = new List<DequeuedMessage>()
            {
                new DequeuedMessage
                {
                    Message = new QueueMessage
                    {
                        StructPayload = payload
                    },
                    LeaseId = "1"
                }
            };

            var queueReceieveResponse = new QueueDequeueResponse();
            queueReceieveResponse.Messages.AddRange(messages);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueDequeueResponse>(Task.FromResult(queueReceieveResponse), null, null, null, null))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            var response = await queue.Dequeue(3);

            Assert.Equal("John Smith", response[0].Message.Name);
            Assert.Equal(30.0, response[0].Message.Age);
            Assert.Equal("123 street st", response[0].Message.Addresses[0]);

            qc.Verify(
                t => t.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDequeueNoMessages()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueDequeueResponse>(Task.FromResult(new QueueDequeueResponse()), null, null, null, null))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            var response = await queue.Dequeue(3);

            Assert.Empty(response);

            qc.Verify(
                t => t.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDequeueToNonExistentQueue()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified queue does not exist")))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            try
            {
                await queue.Dequeue(3);
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified queue does not exist\")",
                    e.Message);
            }

            qc.Verify(
                t => t.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestComplete()
        {
            var payload = new Google.Protobuf.WellKnownTypes.Struct();
            payload.Fields.Add("Name", Value.ForString("John Smith"));
            payload.Fields.Add("Age", Value.ForNumber(30.0));
            payload.Fields.Add("Addresses", Value.ForList(new[] { Value.ForString("123 street st") }));

            var messages = new List<DequeuedMessage>()
            {
                new DequeuedMessage
                {
                    Message = new QueueMessage
                    {
                        StructPayload = payload
                    },
                    LeaseId = "1"
                }
            };

            var queueReceieveResponse = new QueueDequeueResponse();
            queueReceieveResponse.Messages.AddRange(messages);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueDequeueResponse>(Task.FromResult(queueReceieveResponse), null, null, null, null))
                .Verifiable();

            qc.Setup(e => e.CompleteAsync(It.IsAny<QueueCompleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueCompleteResponse>(Task.FromResult(new QueueCompleteResponse()), null, null, null, null))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            var response = await queue.Dequeue(3);

            await response.ToList()[0].Complete();

            qc.Verify(
                t => t.CompleteAsync(It.IsAny<QueueCompleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestCompleteToNonExistentQueue()
        {
            var payload = new Google.Protobuf.WellKnownTypes.Struct();
            payload.Fields.Add("Name", Value.ForString("John Smith"));
            payload.Fields.Add("Age", Value.ForNumber(30.0));
            payload.Fields.Add("Addresses", Value.ForList(new[] { Value.ForString("123 street st") }));

            var messages = new List<DequeuedMessage>()
            {
                new DequeuedMessage
                {
                    Message = new QueueMessage
                    {
                        StructPayload = payload
                    },
                    LeaseId = "1"
                }
            };

            var queueReceieveResponse = new QueueDequeueResponse();
            queueReceieveResponse.Messages.AddRange(messages);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.DequeueAsync(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<QueueDequeueResponse>(Task.FromResult(queueReceieveResponse), null, null, null, null))
                .Verifiable();

            Mock<GrpcClient> qcr = new Mock<GrpcClient>();
            qc.Setup(e => e.CompleteAsync(It.IsAny<QueueCompleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified queue does not exist")))
                .Verifiable();

            var queue = new Sdk.Queue.Queue<TestProfile>("test-queue", qc.Object);

            var response = await queue.Dequeue(3);

            try
            {
                await response.ToList()[0].Complete();
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified queue does not exist\")",
                    e.Message);
            }

            qc.Verify(
                t => t.CompleteAsync(It.IsAny<QueueCompleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
