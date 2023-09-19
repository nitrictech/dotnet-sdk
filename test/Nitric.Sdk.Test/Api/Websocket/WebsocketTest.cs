using System;
using System.Text;
using Google.Protobuf;
using Grpc.Core;
using Moq;
using Nitric.Proto.Secret.v1;
using Nitric.Proto.Websocket.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Resource;
using Nitric.Sdk.Secret;
using Nitric.Sdk.Websocket;
using Xunit;

namespace Nitric.Sdk.Test.Api.Websocket;

public class WebsocketTest {
    [Fact]
    public void TestWebsocketBuild()
    {
        var websocket = new WebsocketClient();
        Assert.NotNull(websocket);
    }

    [Fact]
    public void TestBuildSecretsWithNullClient()
    {
        var websocket = new WebsocketClient(null);
        Assert.NotNull(websocket);
    }

    [Fact]
    public void TestBuildWebsocketWithIdAndSocket()
    {
        var connection = new WebsocketClient().Connection("socket-name", "connection-id");
        Assert.NotNull(connection);
        Assert.Equal("connection-id", connection.Id);
        Assert.Equal("socket-name", connection.Socket);
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
            Socket = "socket-name",
            ConnectionId = "connection-id",
            Data = ByteString.CopyFromUtf8("websocket-data")
        };
        Mock<WebsocketService.WebsocketServiceClient> wc = new Mock<WebsocketService.WebsocketServiceClient>();
        wc.Setup(e =>
                e.Send(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
            .Verifiable();

        var connection = new WebsocketClient(wc.Object)
            .Connection("socket-name", "connection-id");

        connection.Send("websocket-data");

        wc.Verify(
            t => t.Send(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void TestWebsocketSendWithError()
    {
        var websocketSendRequest = new WebsocketSendRequest
        {
            Socket = "socket-name",
            ConnectionId = "connection-id",
            Data = ByteString.CopyFromUtf8("websocket-data")
        };
        Mock<WebsocketService.WebsocketServiceClient> wc = new Mock<WebsocketService.WebsocketServiceClient>();
        wc.Setup(e =>
                e.Send(websocketSendRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
            .Throws(new RpcException(Status.DefaultCancelled, "succeeded in failing"));

        var connection = new WebsocketClient(wc.Object)
            .Connection("socket-name", "connection-id");

        Assert.Throws<CancelledException>(() =>
            connection.Send("websocket-data")
        );
    }

    [Fact]
    public void TestWebsocketClose()
    {
        var websocketCloseRequest = new WebsocketCloseRequest
        {
            Socket = "socket-name",
            ConnectionId = "connection-id",
        };
        Mock<WebsocketService.WebsocketServiceClient> wc = new Mock<WebsocketService.WebsocketServiceClient>();
        wc.Setup(e =>
                e.Close(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
            .Verifiable();

        var connection = new WebsocketClient(wc.Object)
            .Connection("socket-name", "connection-id");

        connection.Close();

        wc.Verify(
            t => t.Close(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void TestWebsocketCloseWithError()
    {
        var websocketCloseRequest = new WebsocketCloseRequest
        {
            Socket = "socket-name",
            ConnectionId = "connection-id",
        };
        Mock<WebsocketService.WebsocketServiceClient> wc = new Mock<WebsocketService.WebsocketServiceClient>();
        wc.Setup(e =>
                e.Close(websocketCloseRequest, null, null, It.IsAny<System.Threading.CancellationToken>()))
            .Throws(new RpcException(Status.DefaultCancelled, "succeeded in failing"));

        var connection = new WebsocketClient(wc.Object)
            .Connection("socket-name", "connection-id");

        Assert.Throws<CancelledException>(() =>
            connection.Close()
        );
    }
}
