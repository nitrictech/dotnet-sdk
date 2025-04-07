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
using Nitric.Proto.Topics.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Service;
using Nitric.Sdk.Topics;
using Nitric.Sdk.Worker;
using Action = Nitric.Proto.Resources.v1.Action;
using ResourceType = Nitric.Proto.Resources.v1.ResourceType;
using GrpcClient = Nitric.Proto.Topics.v1.Topics.TopicsClient;
using System.Threading.Tasks;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for topic resources.
    ///</Summary>
    public enum TopicPermission
    {
        /// <summary>
        /// Enables publishing new events to the topic.
        /// </summary>
        Publish
    }

    public class TopicResource<T> : SecureResource<TopicPermission>
    {
        internal readonly GrpcClient Client;

        internal TopicResource(string name, GrpcClient client = null) : base(name, ResourceType.Topic)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<TopicPermission> permissions)
        {
            var actionMap = new Dictionary<TopicPermission, List<Action>>
            {
                {
                    TopicPermission.Publish,
                    new List<Action> { Action.TopicPublish }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x]))
                .Distinct();
        }

        /// <summary>
        /// Registers a chain of middleware to be called whenever a new event is published to this topic.
        /// </summary>
        /// <param name="middleware">The middleware to call to process events</param>
        public void Subscribe(params Middleware<MessageContext<T>>[] middleware)
        {
            var registrationRequest = new RegistrationRequest { TopicName = this.Name };

            var subWorker = new SubscriptionWorker<T>(registrationRequest, middleware);

            Nitric.RegisterWorker(subWorker);
        }

        /// <summary>
        /// Registers a handler to be called whenever a new event is published to this topic.
        /// </summary>
        /// <param name="handler">The handler to call to process events</param>
        public void Subscribe(Func<MessageContext<T>, Task<MessageContext<T>>> handler)
        {
            var registrationRequest = new RegistrationRequest { TopicName = this.Name };

            var subWorker = new SubscriptionWorker<T>(registrationRequest, handler);

            Nitric.RegisterWorker(subWorker);
        }

        /// <summary>
        /// Request specific access to this topic.
        /// </summary>
        /// <param name="permissions">The permissions that the function has to access the topic.</param>
        /// <returns>A reference to the topic.</returns>
        public Topic<T> Allow(TopicPermission permission, params TopicPermission[] permissions)
        {
            var allPerms = new List<TopicPermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);

            return new Topic<T>(this.Name, this.Client);
        }
    }
}
