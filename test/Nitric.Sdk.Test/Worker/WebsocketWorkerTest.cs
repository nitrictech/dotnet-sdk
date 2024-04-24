using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Moq;
using Nitric.Proto.Websockets.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Xunit;

using GrpcClient = Nitric.Proto.Websockets.v1.WebsocketHandler.WebsocketHandlerClient;

namespace Nitric.Sdk.Test.Worker
{
    public class WebsocketWorkerTest
    {
        [Fact]
        public void TestWebsocketWorkerBuildWithMiddleware()
        {
            Func<WebsocketContext, WebsocketContext> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                SocketName = "websocket-name",
            };

            var worker = new WebsocketWorker(registration, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestWebsocketWorkerBuildWithMultipleMiddleware()
        {
            Middleware<WebsocketContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                SocketName = "websocket-name",
            };

            var worker = new WebsocketWorker(registration, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestWebsocketWorkerBuildWithNoMiddleware()
        {
            Middleware<WebsocketContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                SocketName = "websocket-name",
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new WebsocketWorker(registration);
            });
        }

        [Fact]
        public async void TestWebsocketWorkerStart()
        {
            Middleware<WebsocketContext> middleware = (ctx, next) =>
            {
                Assert.Equal("websocket-name", ctx.Req.SocketName);
                Assert.Equal("connection-1234", ctx.Req.ConnectionId);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                SocketName = "websocket-name",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage 
                    {
                      Id = "id-2",
                      WebsocketEventRequest = new WebsocketEventRequest
                      {
                        SocketName = "websocket-name",
                        ConnectionId = "connection-1234",
                        Connection = new WebsocketConnectionEvent()
                      },
                    },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.HandleEvents(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new WebsocketWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.HandleEvents(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestWebsocketWorkerStartsWithErrors()
        {
            Middleware<WebsocketContext> middleware = (ctx, next) =>
            {
                throw new ApplicationException("Expected test exception!");
            };

            var registration = new RegistrationRequest
            {
                SocketName = "websocket-name",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage
                    {
                      Id = "id-2",
                      WebsocketEventRequest = new WebsocketEventRequest
                      {
                        SocketName = "websocket-name",
                        ConnectionId = "connection-1234",
                        Connection = new WebsocketConnectionEvent()
                      },
                    },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.HandleEvents(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new WebsocketWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.HandleEvents(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}