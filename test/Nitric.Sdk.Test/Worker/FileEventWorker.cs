using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Moq;
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Storage;
using Nitric.Sdk.Worker;
using Xunit;

using GrpcClient = Nitric.Proto.Storage.v1.StorageListener.StorageListenerClient;

namespace Nitric.Sdk.Test.Worker
{
    public class FileEventWorkerTest
    {
        [Fact]
        public void TestFileEventWorkerBuildWithMiddleware()
        {
            Func<FileEventContext, FileEventContext> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                BucketName = "bucket-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var bucket = new StorageClient().Bucket("bucket-name");

            var worker = new FileEventWorker(registration, bucket, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestFileEventWorkerBuildWithMultipleMiddleware()
        {
            Middleware<FileEventContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                BucketName = "bucket-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var bucket = new StorageClient().Bucket("bucket-name");

            var worker = new FileEventWorker(registration, bucket, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestFileEventWorkerBuildWithNoMiddleware()
        {
            Middleware<FileEventContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                BucketName = "bucket-name",
                BlobEventType = Proto.Storage.v1.BlobEventType.Created,
            };

            var bucket = new StorageClient().Bucket("bucket-name");

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new FileEventWorker(registration, bucket);
            });
        }

        [Fact]
        public async void TestFileEventWorkerStartCreated()
        {
            Middleware<FileEventContext> middleware = (ctx, next) =>
            {
                Assert.Equal("test-file", ctx.Req.File.Name);
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

            var bucket = new StorageClient().Bucket("bucket-name");

            var worker = new FileEventWorker(registration, bucket, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Listen(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestFileEventWorkerStartDeleted()
        {
            Middleware<FileEventContext> middleware = (ctx, next) =>
            {
                Assert.Equal("test-file", ctx.Req.File.Name);
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

            var bucket = new StorageClient().Bucket("bucket-name");

            var worker = new FileEventWorker(registration, bucket, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Listen(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestFileEventWorkerStartsWithErrors()
        {
            Middleware<FileEventContext> middleware = (ctx, next) =>
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

            var bucket = new StorageClient().Bucket("bucket-name");

            var worker = new FileEventWorker(registration, bucket, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Listen(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}