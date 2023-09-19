using System;
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.Websocket.v1.WebsocketService.WebsocketServiceClient;

namespace Nitric.Sdk.Websocket
{
    public class WebsocketClient
    {
        internal readonly GrpcClient client;

        public WebsocketClient(GrpcClient client = null)
        {
            this.client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        public Connection Connection(string socket, string connectionId)
        {
            if (string.IsNullOrEmpty(socket))
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }
            return new Connection(this, connectionId, socket);
        }
    }
}
