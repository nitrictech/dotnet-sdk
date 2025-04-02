using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Grpc.Core;
using Moq;
using Nitric.Proto.Topics.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Xunit;

using GrpcClient = Nitric.Proto.Topics.v1.Subscriber.SubscriberClient;

namespace Nitric.Sdk.Test.Worker
{
    public class TestProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Contacts { get; set; }

    }
    public class SubscriptionWorkerTest
    {
        [Fact]
        public void TestSubscriptionWorkerBuildWithMiddleware()
        {
            Func<MessageContext<TestProfile>, MessageContext<TestProfile>> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                TopicName = "topic-name",
            };

            var worker = new SubscriptionWorker<TestProfile>(registration, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestSubscriptionWorkerBuildWithMultipleMiddleware()
        {
            Middleware<MessageContext<TestProfile>> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                TopicName = "topic-name",
            };

            var worker = new SubscriptionWorker<TestProfile>(registration, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestSubscriptionWorkerBuildWithNoMiddleware()
        {
            var registration = new RegistrationRequest
            {
                TopicName = "topic-name",
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new SubscriptionWorker<TestProfile>(registration);
            });
        }

        [Fact]
        public async void TestSubscriptionWorkerStart()
        {
            Middleware<MessageContext<TestProfile>> middleware = (ctx, next) =>
            {
                Assert.Equal("topic-name", ctx.Req.TopicName);
                Assert.Equal("John Smith", ctx.Req.Message.Name);
                Assert.Equal(21, ctx.Req.Message.Age);
                Assert.Equal("john.smith@email.com", ctx.Req.Message.Contacts[0]);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                TopicName = "topic-name",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse() },
                    new ServerMessage
                    {
                        Id = "id-2", MessageRequest = new MessageRequest
                        {
                            TopicName = "topic-name",
                            Message = new TopicMessage
                            {
                                StructPayload = Struct.FromJsonSerializable(new TestProfile
                                {
                                    Name = "John Smith",
                                    Age = 21,
                                    Contacts = new List<string> { "john.smith@email.com" },
                                })
                            },
                        },
                    },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Subscribe(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new SubscriptionWorker<TestProfile>(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Subscribe(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSubscriptionWorkerStartsWithErrors()
        {
            Middleware<MessageContext<TestProfile>> middleware = (ctx, next) =>
            {
                throw new ApplicationException("Expected test exception!");
            };

            var registration = new RegistrationRequest
            {
                TopicName = "topic-name",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage
                    {
                        Id = "id-2", MessageRequest = new MessageRequest
                        {
                            TopicName = "schedule-name",
                            Message = new TopicMessage
                            {
                                StructPayload = Struct.FromJsonSerializable(new TestProfile
                                {
                                    Name = "John Smith",
                                    Age = 21,
                                    Contacts = new List<string> { "john.smith@email.com" },
                                })
                            },
                        },
                    },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Subscribe(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new SubscriptionWorker<TestProfile>(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Subscribe(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}