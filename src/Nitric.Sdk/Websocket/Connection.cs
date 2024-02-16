//// Copyright 2021, Nitric Technologies Pty Ltd.
////
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
////
////      http://www.apache.org/licenses/LICENSE-2.0
////
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//using Google.Protobuf;
//using Grpc.Core;
//using Nitric.Proto.Websocket.v1;
//using Nitric.Sdk.Common;
//using Nitric.Sdk.Resource;

//namespace Nitric.Sdk.Websocket
//{
//    public class Connection
//    {
//        /// <summary>
//        /// The unique connection Id
//        /// </summary>
//        public string Id { get; set; }

//        public string Socket { get; set; }

//        private readonly WebsocketClient websocket;

//        internal Connection(WebsocketClient websocket, string id, string socket)
//        {
//            this.websocket = websocket;
//            this.Id = id;
//            this.Socket = socket;
//        }

//        public void Send(string data)
//        {
//            var request = new WebsocketSendRequest
//            {
//                ConnectionId = this.Id,
//                Socket = this.Socket,
//                Data = ByteString.CopyFromUtf8(data)
//            };
//            try
//            {
//                this.websocket.client.Send(request);
//            }
//            catch (RpcException e)
//            {
//                throw NitricException.FromRpcException(e);
//            }
//        }

//        public void Close()
//        {
//            var request = new WebsocketCloseRequest
//            {
//                ConnectionId = this.Id,
//                Socket = this.Socket,
//            };

//            try
//            {
//                this.websocket.client.Close(request);
//            }
//            catch (RpcException e)
//            {
//                throw NitricException.FromRpcException(e);
//            }
//        }
//    }
//}
