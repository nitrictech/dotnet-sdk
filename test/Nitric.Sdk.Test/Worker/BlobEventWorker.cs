using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Moq;
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Xunit;

using GrpcClient = Nitric.Proto.Storage.v1.StorageListener.StorageListenerClient;

namespace Nitric.Sdk.Test.Worker
{
    public class BlobEventWorkerTest
    {
        [Fact]
        public void TestBlobEventWorkerBuildWithMiddleware()
        {
            Func<BlobEventContext, BlobEventContext> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                BucketName = "bucket-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var worker = new BlobEventWorker(registration, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestBlobEventWorkerBuildWithMultipleMiddleware()
        {
            Middleware<BlobEventContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                BucketName = "bucket-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var worker = new BlobEventWorker(registration, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestBlobEventWorkerBuildWithNoMiddleware()
        {
            Middleware<BlobEventContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                BucketName = "bucket-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new BlobEventWorker(registration);
            });
        }

        [Fact]
        public async void TestBlobEventWorkerStartCreated()
        {
            Middleware<BlobEventContext> middleware = (ctx, next) =>
            {
                Assert.Equal("test-file", ctx.Req.Key);
                Assert.Equal(Service.BlobEventType.Write, ctx.Req.NotificationType);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                BucketName = "BlobEvent-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", BlobEventRequest = new Proto.Storage.v1.BlobEventRequest { BucketName = "bucket-name", BlobEvent = new BlobEvent { Key="test-file", Type=Proto.Storage.v1.BlobEventType.Created} } }, }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Listen(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new BlobEventWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Listen(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestBlobEventWorkerStartDeleted()
        {
            Middleware<BlobEventContext> middleware = (ctx, next) =>
            {
                Assert.Equal("test-file", ctx.Req.Key);
                Assert.Equal(Service.BlobEventType.Delete, ctx.Req.NotificationType);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                BucketName = "BlobEvent-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", BlobEventRequest = new Proto.Storage.v1.BlobEventRequest { BucketName = "bucket-name", BlobEvent = new BlobEvent { Key="test-file", Type=Proto.Storage.v1.BlobEventType.Deleted} } }, }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Listen(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new BlobEventWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Listen(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestBlobEventWorkerStartsWithErrors()
        {
            Middleware<BlobEventContext> middleware = (ctx, next) =>
            {
                throw new ApplicationException("Expected test exception!");
            };

            var registration = new RegistrationRequest
            {
                BucketName = "BlobEvent-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", BlobEventRequest = new Proto.Storage.v1.BlobEventRequest { BucketName = "bucket-name", BlobEvent = new BlobEvent { Key="test-file", Type=Proto.Storage.v1.BlobEventType.Created} } }, }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Listen(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new BlobEventWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Listen(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}