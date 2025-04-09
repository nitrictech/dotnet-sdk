using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Moq;
using Nitric.Proto.Batch.v1;
using Nitric.Sdk.Common;
using Xunit;
using GrpcClient = Nitric.Proto.Batch.v1.Batch.BatchClient;


namespace Nitric.Sdk.Test.Job
{
    public class TestSubmission
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public List<string> Tags { get; set; }
    }
    public class JobTest
    {
        [Fact]
        public void TestBuildJobWithName()
        {
            var job = new Sdk.Job.Job<TestSubmission>("job-name");

            Assert.NotNull(job);
            Assert.Equal("job-name", job.Name);
        }

        [Fact]
        public void TestBuildJobWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Job.Job<TestSubmission>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Job.Job<TestSubmission>(null)
            );
        }

        [Fact]
        public void TestJobToString()
        {
            var job = new Sdk.Job.Job<TestSubmission>("job-name");

            Assert.Equal("Job`1[name=job-name]", job.ToString());
        }

        [Fact]
        public async Task TestSubmitJob()
        {
            var testSubmission = new TestSubmission
            {
                Id = 1234,
                Message = "This seems like a good test string?",
                Tags = new List<string> { "message", "test" }
            };

            var payload = Sdk.Common.Struct.FromJsonSerializable(testSubmission);

            var request = new JobSubmitRequest
            {
                JobName = "job-name",
                Data = new JobData
                {
                    Struct = payload
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SubmitJobAsync(It.IsAny<JobSubmitRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<JobSubmitResponse>(Task.FromResult(new JobSubmitResponse()), null, null, null, null, null))
                .Verifiable();

            var job = new Sdk.Job.Job<TestSubmission>("job-name", gc.Object);

            await job.Submit(testSubmission);

            gc.Verify(
                t => t.SubmitJobAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestSubmitJobWithNullPayload()
        {
            var request = new JobSubmitRequest
            {
                JobName = "job-name",
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SubmitJobAsync(It.IsAny<JobSubmitRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<JobSubmitResponse>(Task.FromResult(new JobSubmitResponse()), null, null, null, null, null))
                .Verifiable();

            var kv = new Sdk.Job.Job<TestSubmission>("job-name", gc.Object);

            await kv.Submit(null);

            gc.Verify(
                t => t.SubmitJobAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TestSubmitJobWithError()
        {
            var testSubmission = new TestSubmission
            {
                Id = 1234,
                Message = "This seems like a good test string?",
                Tags = new List<string> { "message", "test" }
            };

            var payload = Sdk.Common.Struct.FromJsonSerializable(testSubmission);

            var request = new JobSubmitRequest
            {
                JobName = "job-name",
                Data = new JobData
                {
                    Struct = payload
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SubmitJobAsync(It.IsAny<JobSubmitRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified job does not exist")))
                .Verifiable();

            var job = new Sdk.Job.Job<TestSubmission>("job-name", gc.Object);

            try
            {
                await job.Submit(testSubmission);
                Assert.Fail();
            }
            catch (RpcException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified job does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.SubmitJobAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
