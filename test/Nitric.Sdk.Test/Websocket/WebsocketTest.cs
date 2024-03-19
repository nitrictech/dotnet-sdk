using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Moq;
using Nitric.Proto.Websockets.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Websocket;
using Xunit;
using GrpcClient = Nitric.Proto.Websockets.v1.Websocket.WebsocketClient;

namespace Nitric.Sdk.Test.Websocket
{

    public class WebsocketTest
    {
        [Fact]
        public void TestWebsocketBuild()
        {
            var websocket = new WebsocketClient();
            Assert.NotNull(websocket);
        }

        [Fact]
        public void TestBuildWebsocketsWithNullClient()
        {
            var websocket = new WebsocketClient(null);
            Assert.NotNull(websocket);
        }

        [Fact]
        public void TestBuildWebsocketWithIdAndSocket()
        {
            var connection = new WebsocketClient(null).Connection("socket-name", "connection-id");
            Assert.NotNull(connection);
            Assert.Equal("connection-id", connection.Id);
            Assert.Equal("socket-name", connection.SocketName);
        }

        [Fact]
        public void TestBuildWebsocketWithoutIdOrSocket()
        {
            Assert.Throws<ArgumentNullException>(
                () => new WebsocketClient().Connection("", "connection-id"));
            Assert.Throws<ArgumentNullException>(
                () => new WebsocketClient().Connection(null, "connection-id"));
            Assert.Throws<ArgumentNullException>(
                () => new WebsocketClient().Connection("socket-name", ""));
            Assert.Throws<ArgumentNullException>(
                () => new WebsocketClient().Connection("socket-name", null));
        }

        //Testing Websocket Methods
        [Fact]
        public void TestWebsocketSend()
        {
            var websocketSendRequest = new WebsocketSendRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
                Data = ByteString.CopyFromUtf8("websocket-data")
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.SendMessage(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Verifiable();

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            connection.SendMessage("websocket-data");

            wc.Verify(
                t => t.SendMessage(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestWebsocketSendWithError()
        {
            var websocketSendRequest = new WebsocketSendRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
                Data = ByteString.CopyFromUtf8("websocket-data")
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.SendMessage(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(Status.DefaultCancelled, "succeeded in failing"));

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            Assert.Throws<CancelledException>(() =>
                connection.SendMessage("websocket-data")
            );
        }

        [Fact]
        public async void TestWebsocketSendAsync()
        {
            var websocketSendRequest = new WebsocketSendRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
                Data = ByteString.CopyFromUtf8("websocket-data")
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.SendMessageAsync(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<WebsocketSendResponse>(Task.FromResult(new WebsocketSendResponse()), null, null, null, null))
                .Verifiable();

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            await connection.SendMessageAsync("websocket-data");

            wc.Verify(
                t => t.SendMessageAsync(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestWebsocketSendWithErrorAsync()
        {
            var websocketSendRequest = new WebsocketSendRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
                Data = ByteString.CopyFromUtf8("websocket-data")
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.SendMessageAsync(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(Status.DefaultCancelled, "succeeded in failing"));

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            Assert.ThrowsAsync<CancelledException>(() =>
                connection.SendMessageAsync("websocket-data")
            );
        }

        [Fact]
        public void TestWebsocketClose()
        {
            var websocketCloseRequest = new WebsocketCloseConnectionRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.CloseConnection(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Verifiable();

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            connection.CloseConnection();

            wc.Verify(
                t => t.CloseConnection(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestWebsocketCloseWithError()
        {
            var websocketCloseRequest = new WebsocketCloseConnectionRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.CloseConnection(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(Status.DefaultCancelled, "succeeded in failing"));

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            Assert.Throws<CancelledException>(() =>
                connection.CloseConnection()
            );
        }

        [Fact]
        public async void TestWebsocketCloseAsync()
        {
            var websocketCloseRequest = new WebsocketCloseConnectionRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.CloseConnectionAsync(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<WebsocketCloseConnectionResponse>(Task.FromResult(new WebsocketCloseConnectionResponse()), null, null, null, null))
                .Verifiable();

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            await connection.CloseConnectionAsync();

            wc.Verify(
                t => t.CloseConnectionAsync(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestWebsocketCloseWithErrorAsync()
        {
            var websocketCloseRequest = new WebsocketCloseConnectionRequest
            {
                SocketName = "socket-name",
                ConnectionId = "connection-id",
            };
            Mock<GrpcClient> wc = new Mock<GrpcClient>();
            wc.Setup(e =>
                    e.CloseConnectionAsync(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(Status.DefaultCancelled, "succeeded in failing"));

            var connection = new WebsocketClient(wc.Object)
                .Connection("socket-name", "connection-id");

            Assert.ThrowsAsync<CancelledException>(() =>
                connection.CloseConnectionAsync()
            );
        }
    }
}