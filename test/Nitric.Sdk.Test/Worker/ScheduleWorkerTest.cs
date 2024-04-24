using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Moq;
using Nitric.Proto.Schedules.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Xunit;

using GrpcClient = Nitric.Proto.Schedules.v1.Schedules.SchedulesClient;

namespace Nitric.Sdk.Test.Worker
{
    public class ScheduleWorkerTest
    {
        [Fact]
        public void TestScheduleWorkerBuildWithMiddleware()
        {
            Func<IntervalContext, IntervalContext> middleware = (ctx) =>
            {
                return ctx;
            };

            var registration = new RegistrationRequest
            {
                ScheduleName = "schedule-name",
                Cron = new ScheduleCron { Expression = "* * * * *" }
            };

            var worker = new ScheduleWorker(registration, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestScheduleWorkerBuildWithMultipleMiddleware()
        {
            Middleware<IntervalContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                ScheduleName = "schedule-name",
                Cron = new ScheduleCron { Expression = "* * * * *" }
            };

            var worker = new ScheduleWorker(registration, middleware, middleware);

            Assert.NotNull(worker);
        }

        [Fact]
        public void TestScheduleWorkerBuildWithNoMiddleware()
        {
            Middleware<IntervalContext> middleware = (ctx, next) =>
            {
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                ScheduleName = "schedule-name",
                Cron = new ScheduleCron { Expression = "* * * * *" }
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var worker = new ScheduleWorker(registration);
            });
        }

        [Fact]
        public async void TestScheduleWorkerStart()
        {
            Middleware<IntervalContext> middleware = (ctx, next) =>
            {
                Assert.Equal("schedule-name", ctx.Req.ScheduleName);
                return next(ctx);
            };

            var registration = new RegistrationRequest
            {
                ScheduleName = "schedule-name",
                Cron = new ScheduleCron { Expression = "* * * * *" }
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", IntervalRequest = new Proto.Schedules.v1.IntervalRequest { ScheduleName = "schedule-name" } },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Schedule(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new ScheduleWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Schedule(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestScheduleWorkerStartsWithErrors()
        {
            Middleware<IntervalContext> middleware = (ctx, next) =>
            {
                throw new ApplicationException("Expected test exception!");
            };

            var registration = new RegistrationRequest
            {
                ScheduleName = "schedule-name",
                Cron = new ScheduleCron { Expression = "* * * * *" }
            };

            var mockClientMessage = new FakeClientStreamWriter<ClientMessage>();

            var responseStream = new FakeAsyncStreamReader<ServerMessage>(
                new List<ServerMessage>
                {
                    new ServerMessage { Id = "id-1", RegistrationResponse = new RegistrationResponse { }},
                    new ServerMessage { Id = "id-2", IntervalRequest = new Proto.Schedules.v1.IntervalRequest { ScheduleName = "schedule-name" } },
                }
            );

            var resp = new AsyncDuplexStreamingCall<ClientMessage, ServerMessage>(mockClientMessage, responseStream, null, null, null, null);

            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.Schedule(null, null, It.IsAny<CancellationToken>()))
                .Returns(resp);

            var worker = new ScheduleWorker(registration, middleware)
            {
                GrpcClient = wc.Object
            };

            await worker.Start();

            wc.Verify(
                t => t.Schedule(null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}