using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Moq;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Nitric.Proto.Batch.v1;
using Xunit;

using GrpcClient = Nitric.Proto.Batch.v1.Job.JobClient;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Test.Worker
{
    public class TestSubmission
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public List<string> Tags { get; set; }
    }

    public class JobWorkerTest
    {
        [Fact]
        public void TestJobWorkerBuildWithMiddleware()
        {
            Func<JobContext<TestSubmission>, JobContext<TestSubmission>> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                JobName = "job-name",
            };

            var worker = new JobWorker<TestSubmission>(registration, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestJobWorkerBuildWithMultipleMiddleware()
        {
            Middleware<JobContext<TestSubmission>> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                JobName = "job-name"
            };

            var worker = new JobWorker<TestSubmission>(registration, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestJobWorkerBuildWithNoMiddleware()
        {
            Middleware<JobContext<TestSubmission>> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                JobName = "job-name",
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new JobWorker<TestSubmission>(registration);
            });
        }

        [Fact]
        public async void TestJobWorkerStart()
        {
            var testSubmission = new TestSubmission
            {
                Id = 1234,
                Message = "this seems like a good test message",
                Tags = new List<string> { "message", "test" }
            };

            var jobRequest = new JobRequest
            {
                JobName = "schedule-name",
                Data = new JobData
                {
                    Struct = Struct.FromJsonSerializable(testSubmission),
                }
            };

            Middleware<JobContext<TestSubmission>> middleware = (ctx, next) =>
            {
                Assert.Equal("job-name", ctx.Req.JobName);
                Assert.Equal(1234, ctx.Req.Data.Id);
                Assert.Equal("this seems like a good test message", ctx.Req.Data.Message);
                Assert.Equal(new List<string> { "message", "test" }, ctx.Req.Data.Tags);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                JobName = "job-name",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", JobRequest = jobRequest },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.HandleJob(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new JobWorker<TestSubmission>(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.HandleJob(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestScheduleWorkerStartsWithErrors()
        {
            var testSubmission = new TestSubmission
            {
                Id = 1234,
                Message = "this seems like a good test message",
                Tags = new List<string> { "message", "test" }
            };

            var jobRequest = new JobRequest
            {
                JobName = "schedule-name",
                Data = new JobData
                {
                    Struct = Struct.FromJsonSerializable(testSubmission),
                }
            };

            Middleware<JobContext<TestSubmission>> middleware = (ctx, next) =>
            {
                throw new ApplicationException("Expected test exception!");
            };

            var registration = new RegistrationRequest
            {
                JobName = "job-name",
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", JobRequest = jobRequest },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.HandleJob(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new JobWorker<TestSubmission>(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.HandleJob(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}