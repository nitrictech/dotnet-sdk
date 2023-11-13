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
using System.Runtime.CompilerServices;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Function;
using Nitric.Sdk.Websocket;
using Action = Nitric.Proto.Resource.v1.Action;
using NitricResource = Nitric.Proto.Resource.v1.Resource;
using GrpcClient = Nitric.Proto.Websocket.v1.WebsocketService.WebsocketServiceClient;

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
            var resource = new NitricResource { Name = this.Name, Type = ResourceType.Websocket };
            var request = new ResourceDeclareRequest { Resource = resource };
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
        /// <param name="notificationType">The type of websocket event listener</param>
        /// <param name="middleware">The middleware to call to process events</param>
        public void On(WebsocketEventType notificationType, params Middleware<WebsocketContext>[] middleware)
        {
            var websocketWorker = new Faas(new WebsocketWorkerOptions(this.Name, notificationType));

            websocketWorker.Websocket(middleware);

            Nitric.RegisterWorker(websocketWorker);
        }

        /// <summary>
        /// Registers a handler to be called whenever a new event is published to this websocket.
        /// </summary>
        /// <param name="notificationType">The type of websocket event</param>
        /// <param name="handler">The handler to call to process websocket events</param>
        public void On(WebsocketEventType notificationType, Func<WebsocketContext, WebsocketContext> handler)
        {
            var websocketWorker = new Faas(new WebsocketWorkerOptions(this.Name, notificationType));

            websocketWorker.Websocket(handler);

            Nitric.RegisterWorker(websocketWorker);
        }

        public Connection Connection(string connectionId)
        {
            return this.wsClient.Connection(this.Name, connectionId);
        }
    }
}
