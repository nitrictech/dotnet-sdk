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
using Nitric.Sdk.Queue;
using Xunit;

namespace Nitric.Sdk.Test.Api.Queue
{
    public class QueueClientTest
    {
        [Fact]
        public void TestBuildQueues()
        {
            var queues = new QueuesClient();
            Assert.NotNull(queues);
        }

        [Fact]
        public void TestBuildQueueWithName()
        {
            var queue = new QueuesClient().Queue<TestProfile>("test-queue");
            Assert.NotNull(queue);
            Assert.Equal("test-queue", queue.Name);
        }

        [Fact]
        public void TestBuildQueueWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueuesClient().Queue<TestProfile>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new QueuesClient().Queue<TestProfile>(null)
            );
        }

        [Fact]
        public void TestSendToNonExistentQueue()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e =>
                    e.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified queue does not exist")))
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            try
            {
                queue.Enqueue(new TestProfile());
            }
            catch (Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified queue does not exist\")",
                    ne.Message);
            }

            qc.Verify(
                t => t.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestSendBatchWithFailedTasks()
        {
            FailedEnqueueMessage failedMessage = new FailedEnqueueMessage();
            failedMessage.Details = "I am a failed task... I failed my task";
            failedMessage.Message = new QueueMessage();

            List<FailedEnqueueMessage> failedMessages = new List<FailedEnqueueMessage>
            {
                failedMessage,
            };

            var queueBatchResponse = new QueueEnqueueResponse();
            queueBatchResponse.FailedMessages.AddRange(failedMessages);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueBatchResponse)
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            var failedTasksResp = queue.Enqueue(new TestProfile { }, new TestProfile { });

            Assert.Equal("I am a failed task... I failed my task", failedTasksResp[0].Details);

            qc.Verify(
                t => t.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestSendBatchWithNoFailedTasks()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueEnqueueResponse())
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            var failedTasks = queue.Enqueue(new TestProfile { }, new TestProfile { });

            Assert.Empty(failedTasks);

            qc.Verify(
                t => t.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestReceiveTasks()
        {
            var messagePayload = Common.Struct.FromJsonSerializable(new TestProfile { Name = "John Smith" });
            var message = new DequeuedMessage
            {
                LeaseId = "1234",
                Message = new QueueMessage { StructPayload = messagePayload },
            };

            var messages = new List<DequeuedMessage>() { message };

            var queueReceieveResponse = new QueueDequeueResponse();
            queueReceieveResponse.Messages.AddRange(messages);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.Dequeue(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueReceieveResponse)
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            var response = queue.Dequeue(3);

            Assert.Equal("1234", response[0].LeaseId);
            Assert.Equal("John Smith", response[0].Message.Name);

            qc.Verify(
                t => t.Dequeue(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestReceiveNoTasks()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.Dequeue(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueDequeueResponse())
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            var response = queue.Dequeue(3);

            Assert.Empty(response);

            qc.Verify(
                t => t.Dequeue(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestSend()
        {
            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e =>
                    e.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueEnqueueResponse())
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            queue.Enqueue(new TestProfile { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } });

            qc.Verify(
                t => t.Enqueue(It.IsAny<QueueEnqueueRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestComplete()
        {
            var receivedTask = new ReceivedMessage<TestProfile>
            {
                LeaseId = "1",
                Message = new TestProfile { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } }
            };

            var payload = new Struct();
            payload.Fields.Add("Name", Value.ForString("John Smith"));
            payload.Fields.Add("Age", Value.ForNumber(30.0));
            payload.Fields.Add("Addresses", Value.ForList(new[] { Value.ForString("123 street st") }));

            var tasks = new List<DequeuedMessage>()
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
            queueReceieveResponse.Messages.AddRange(tasks);

            Mock<GrpcClient> qc = new Mock<GrpcClient>();
            qc.Setup(e => e.Dequeue(It.IsAny<QueueDequeueRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueReceieveResponse)
                .Verifiable();

            Mock<GrpcClient> qcr = new Mock<GrpcClient>();
            qc.Setup(e => e.Complete(It.IsAny<QueueCompleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Verifiable();

            var queue = new QueuesClient(qc.Object).Queue<TestProfile>("test-queue");

            var response = queue.Dequeue(3);

            response.ToList()[0].Complete();

            qc.Verify(
                t => t.Complete(It.IsAny<QueueCompleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
