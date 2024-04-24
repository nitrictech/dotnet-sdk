using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using Grpc.Core;
using Moq;
using Nitric.Proto.Apis.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Xunit;

using GrpcClient = Nitric.Proto.Apis.v1.Api.ApiClient;

namespace Nitric.Sdk.Test.Worker
{
    public class ApiWorkerTest
    {
        [Fact]
        public void TestApiWorkerBuildWithMiddleware()
        {
            Func<HttpContext, HttpContext> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                Api = "api-name",
                Path = "/",
            };

            var worker = new ApiWorker(registration, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestApiWorkerBuildWithMultipleMiddleware()
        {
            Middleware<HttpContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                Api = "api-name",
                Path = "/",
            };

            var worker = new ApiWorker(registration, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestApiWorkerBuildWithNoMiddleware()
        {
            Middleware<HttpContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                Api = "api-name",
                Path = "/",
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new ApiWorker(registration);
            });
        }

        [Fact]
        public async void TestApiWorkerStart()
        {
            Middleware<HttpContext> middleware = (ctx, next) =>
            {
                var profile = ctx.Req.Json<TestProfile>();
                Assert.Equal("John Smith", profile.Name);
                Assert.Equal(21, profile.Age);
                Assert.Equal("john.smith@email.com", profile.Contacts[0]);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                Api = "api-name",
                Path = "/",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var body = ByteString.CopyFromUtf8("{\"name\":\"John Smith\",\"age\":21,\"contacts\":[\"john.smith@email.com\"]}");

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", HttpRequest = new Proto.Apis.v1.HttpRequest { Body = body} },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Serve(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new ApiWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Serve(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestScheduleWorkerStartsWithErrors()
        {
            Middleware<HttpContext> middleware = (ctx, next) =>
            {
                throw new ApplicationException("Expected test exception!");
            };

            var registration = new RegistrationRequest
            {
                Api = "api-name",
                Path = "/",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var body = ByteString.CopyFromUtf8("{\"name\":\"John Smith\",\"age\":21,\"contacts\":[\"john.smith@email.com\"]}");

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", HttpRequest = new Proto.Apis.v1.HttpRequest { Body = body} },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Serve(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new ApiWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Serve(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}