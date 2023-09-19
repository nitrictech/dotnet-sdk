using Google.Protobuf;
using Grpc.Core;
using Nitric.Proto.Websocket.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Resource;

namespace Nitric.Sdk.Websocket
{
    public class Connection
    {
        /// <summary>
        /// The unique connection Id
        /// </summary>
        public string Id { get; set; }

        public string Socket { get; set; }

        private readonly WebsocketClient websocket;

        internal Connection(WebsocketClient websocket, string id, string socket)
        {
            this.websocket = websocket;
            this.Id = id;
            this.Socket = socket;
        }

        public void Send(string data)
        {
            var request = new WebsocketSendRequest
            {
                ConnectionId = this.Id,
                Socket = this.Socket,
                Data = ByteString.CopyFromUtf8(data)
            };
            try
            {
                this.websocket.client.Send(request);
            }
            catch (RpcException e)
            {
                throw NitricException.FromRpcException(e);
            }
        }

        public void Close()
        {
            var request = new WebsocketCloseRequest
            {
                ConnectionId = this.Id,
                Socket = this.Socket,
            };

            try
            {
                this.websocket.client.Close(request);
            }
            catch (RpcException e)
            {
                throw NitricException.FromRpcException(e);
            }
        }
    }
}
