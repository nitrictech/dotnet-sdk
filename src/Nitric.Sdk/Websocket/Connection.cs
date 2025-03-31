// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Nitric.Proto.Websockets.v1;
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.Websockets.v1.Websocket.WebsocketClient;

namespace Nitric.Sdk.Websocket
{
    public class Connection
    {
        /// <summary>
        /// The unique connection Id
        /// </summary>
        public string Id { get; set; }

        public string SocketName { get; set; }

        private readonly GrpcClient Client;

        internal Connection(GrpcClient client, string connectionId, string socketName)
        {
            this.Client = client;
            this.Id = connectionId;
            this.SocketName = socketName;
        }

        /// <summary>
        /// Send a message to the websocket.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        public void SendMessage(string message)
        {
            var request = new WebsocketSendRequest
            {
                ConnectionId = this.Id,
                SocketName = this.SocketName,
                Data = ByteString.CopyFromUtf8(message)
            };
            try
            {
                this.Client.SendMessage(request);
            }
            catch (RpcException e)
            {
                throw NitricException.FromRpcException(e);
            }
        }

        /// <summary>
        /// Send a message to the websocket.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        public async Task SendMessageAsync(string message)
        {
            var request = new WebsocketSendRequest
            {
                ConnectionId = this.Id,
                SocketName = this.SocketName,
                Data = ByteString.CopyFromUtf8(message)
            };
            try
            {
                await this.Client.SendMessageAsync(request);
            }
            catch (RpcException e)
            {
                throw NitricException.FromRpcException(e);
            }
        }

        /// <summary>
        /// Close the connection to the websocket.
        /// </summary>
        public void CloseConnection()
        {
            var request = new WebsocketCloseConnectionRequest
            {
                ConnectionId = this.Id,
                SocketName = this.SocketName,
            };

            try
            {
                this.Client.CloseConnection(request);
            }
            catch (RpcException e)
            {
                throw NitricException.FromRpcException(e);
            }
        }

        /// <summary>
        /// Close the connection to the websocket.
        /// </summary>
        public async Task CloseConnectionAsync()
        {
            var request = new WebsocketCloseConnectionRequest
            {
                ConnectionId = this.Id,
                SocketName = this.SocketName,
            };

            try
            {
                await this.Client.CloseConnectionAsync(request);
            }
            catch (RpcException e)
            {
                throw NitricException.FromRpcException(e);
            }
        }
    }
}
