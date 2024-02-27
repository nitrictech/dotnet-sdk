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
using Nitric.Proto.Resources.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Service;
using Nitric.Sdk.Websocket;
using Action = Nitric.Proto.Resources.v1.Action;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;
using GrpcClient = Nitric.Proto.Websockets.v1.Websocket.WebsocketClient;
using Nitric.Sdk.Worker;
using Nitric.Proto.Websockets.v1;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for websocket resources.
    ///</Summary>
    public enum WebsocketPermission
    {
        /// <summary>
        /// Enables pushing new events to the websocket.
        /// </summary>
        Manage
    }

    public class WebsocketResource : SecureResource<WebsocketPermission>
    {
        private readonly WebsocketClient wsClient;

        internal WebsocketResource(string name) : base(name, ResourceType.Websocket)
        {
            this.wsClient = new WebsocketClient(new GrpcClient(GrpcChannelProvider.GetChannel()));
        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<WebsocketPermission> permissions)
        {
            var actionMap = new Dictionary<WebsocketPermission, List<Action>>
            {
                {
                    WebsocketPermission.Manage,
                    new List<Action> { Action.WebsocketManage }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x]))
                .Distinct();
        }

        /// <summary>
        /// Registers a chain of middleware to be called whenever a new event is published to this topic.
        /// </summary>
        /// <param name="eventType">The type of websocket event listener</param>
        /// <param name="middleware">The middleware to call to process events</param>
        public void On(Service.WebsocketEventType eventType, params Middleware<WebsocketContext>[] middlewares)
        {
            var registrationRequest = new RegistrationRequest
            {
                SocketName = Name,
                EventType = eventType.ToGrpc()
            };

            var websocketWorker = new WebsocketWorker(registrationRequest, middlewares);

            Nitric.RegisterWorker(websocketWorker);
        }

        /// <summary>
        /// Registers a handler to be called whenever a new event is published to this websocket.
        /// </summary>
        /// <param name="eventType">The type of websocket event</param>
        /// <param name="handler">The handler to call to process websocket events</param>
        public void On(Service.WebsocketEventType eventType, Func<WebsocketContext, WebsocketContext> handler)
        {
            var registrationRequest = new RegistrationRequest
            {
                SocketName = Name,
                EventType = eventType.ToGrpc()
            };

            var websocketWorker = new WebsocketWorker(registrationRequest, handler);

            Nitric.RegisterWorker(websocketWorker);
        }

        public Connection Connection(string connectionId)
        {
            return this.wsClient.Connection(this.Name, connectionId);
        }
    }
}
