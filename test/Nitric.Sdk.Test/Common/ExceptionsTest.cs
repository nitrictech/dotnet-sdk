using Nitric.Sdk.Common;
using Grpc.Core;
using Xunit;

namespace Nitric.Sdk.Test.Common
{
    public class ExceptionsTest
    {
        [Fact]
        public void BuildUnknownException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Unknown, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Unknown\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildCancelledException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Cancelled, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Cancelled\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildInvalidArgumentException()
        {
            var rpcException = new RpcException(new Status(StatusCode.InvalidArgument, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"InvalidArgument\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildDeadlineExceededException()
        {
            var rpcException = new RpcException(new Status(StatusCode.DeadlineExceeded, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"DeadlineExceeded\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildNotFoundException()
        {
            var rpcException = new RpcException(new Status(StatusCode.NotFound, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildAlreadyExistsException()
        {
            var rpcException = new RpcException(new Status(StatusCode.AlreadyExists, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"AlreadyExists\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildPermissionDeniedException()
        {
            var rpcException = new RpcException(new Status(StatusCode.PermissionDenied, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"PermissionDenied\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildResourceExhaustedException()
        {
            var rpcException = new RpcException(new Status(StatusCode.ResourceExhausted, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"ResourceExhausted\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildFailedPreconditionException()
        {
            var rpcException = new RpcException(new Status(StatusCode.FailedPrecondition, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"FailedPrecondition\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildAbortedException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Aborted, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Aborted\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildOutOfRangeException()
        {
            var rpcException = new RpcException(new Status(StatusCode.OutOfRange, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"OutOfRange\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildUnimplementedException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Unimplemented, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Unimplemented\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildInternalException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Internal, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Internal\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildUnavailableException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Unavailable, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Unavailable\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildDataLossException()
        {
            var rpcException = new RpcException(new Status(StatusCode.DataLoss, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"DataLoss\", Detail=\"an error occurred\")",
                    exception.Message);
        }

        [Fact]
        public void BuildUnauthenticatedException()
        {
            var rpcException = new RpcException(new Status(StatusCode.Unauthenticated, "an error occurred"));

            var exception = NitricException.FromRpcException(rpcException);

            Assert.Equal("Status(StatusCode=\"Unauthenticated\", Detail=\"an error occurred\")",
                    exception.Message);
        }
    }
}

