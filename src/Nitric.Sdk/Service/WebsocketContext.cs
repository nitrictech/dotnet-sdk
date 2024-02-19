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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Nitric.Proto.Websockets.v1;
using WebsocketEventTypeProto = Nitric.Proto.Websockets.v1.WebsocketEventRequest.WebsocketEventOneofCase;
using Google.Protobuf.Collections;
using ProtoWebsocketEventType = Nitric.Proto.Websockets.v1.WebsocketEventType;

namespace Nitric.Sdk.Service
{
    /// <summary>
    /// Extension class for methods converting blob event types to and from gRPC calls.
    /// </summary>
    internal static class WebsocketEventTypeExtension
    {
        /// <summary>
        /// Convert a SDK websocket event type into a gRPC blob event type.
        /// </summary>
        /// <param name="websocketEventType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static ProtoWebsocketEventType ToGrpc(
            this WebsocketEventType websocketEventType)
        {
            return websocketEventType switch
            {
                WebsocketEventType.Connected => ProtoWebsocketEventType.Connect,
                WebsocketEventType.Disconnected => ProtoWebsocketEventType.Disconnect,
                WebsocketEventType.Message => ProtoWebsocketEventType.Message,
                _ => throw new ArgumentException("Unsupported websocket event type")
            };
        }
    }

    public enum WebsocketEventType
    {
        Connected,
        Disconnected,
        Message
    }

    public class WebsocketRequest : TriggerRequest
    {
        /// <summary>
        /// The raw message bytes
        /// </summary>
        private byte[] data;

        /// <summary>
        /// The name of the websocket that triggered this request
        /// </summary>
        public string SocketName { get; private set; }

        /// <summary>
        /// The type of websocket notification
        /// </summary>
        public WebsocketEventType NotificationType { get; private set; }

        /// <summary>
        /// The ID of the individual connection
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// The websocket query parameters
        /// </summary>
        public Dictionary<string, string[]> Query { get; private set; }

        public T Message<T>() => JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(this.data));

        public string Message() => Encoding.Default.GetString(this.data);

        /// <summary>
        /// Construct an event request
        /// </summary>
        /// <param name="socketName">the source websocket</param>
        /// <param name="notificationType">the type of websocket notification</param>
        /// <param name="connectionId">the id of the individual connection</param>
        /// <param name="query">the websocket query parameters </param>
        public WebsocketRequest(byte[] data, string socketName, WebsocketEventType notificationType, string connectionId, Dictionary<string, string[]> query) : base()
        {
            this.data = data;
            this.SocketName = socketName;
            this.NotificationType = notificationType;
            this.ConnectionId = connectionId;
            this.Query = query;
        }
    }

    public class WebsocketResponse : TriggerResponse
    {
        /// <summary>
        /// Indicates whether the event was successfully processed.
        ///
        /// If this value is false, the event may be resent.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Construct an event response.
        /// </summary>
        /// <param name="success">Indicates whether the event was successfully processed.</param>
        public WebsocketResponse(bool success)
        {
            this.Success = success;
        }
    }

    public class WebsocketContext : TriggerContext<WebsocketRequest, WebsocketResponse>
    {
        /// <summary>
        /// Construct a new Websocket Context.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public WebsocketContext(string id, WebsocketRequest req, WebsocketResponse res) : base(id, req, res)
        {
        }

        /// <summary>
        /// Construct an event topic from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into an EventContext.</param>
        /// <returns>the new event context</returns>
        public static WebsocketContext FromRequest(ServerMessage trigger)
        {
            var type = FromGrpcWebsocketNotificationType(trigger.WebsocketEventRequest.WebsocketEventCase);
            var queryParams = GetQueryParams(trigger.WebsocketEventRequest.Connection.QueryParams);

            return new WebsocketContext(
                trigger.Id,
                new WebsocketRequest(
                    trigger.WebsocketEventRequest.Message.Body.ToByteArray(),
                    trigger.WebsocketEventRequest.SocketName,
                    type,
                    trigger.WebsocketEventRequest.ConnectionId,
                    queryParams
                ),
                new WebsocketResponse(true));
        }

        private static Dictionary<string, string[]> GetQueryParams(MapField<string, QueryValue> queryParams)
        {
            return queryParams.Select
                (kv =>
                    new KeyValuePair<string, string[]>(kv.Key, kv.Value.Value.ToArray())
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static WebsocketEventType FromGrpcWebsocketNotificationType(
            WebsocketEventTypeProto notificationType)
        {
            return notificationType switch
            {
                WebsocketEventTypeProto.Connection => WebsocketEventType.Connected,
                WebsocketEventTypeProto.Disconnection => WebsocketEventType.Disconnected,
                WebsocketEventTypeProto.Message => WebsocketEventType.Message,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        public ClientMessage ToResponse()
        {
            return new ClientMessage { Id = Id, WebsocketEventResponse = new WebsocketEventResponse { } };
        }
    }
}
