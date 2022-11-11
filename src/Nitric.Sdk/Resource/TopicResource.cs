using System;
using System.Collections.Generic;
using System.Linq;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Event;
using Nitric.Sdk.Function;
using Action = Nitric.Proto.Resource.v1.Action;
using NitricResource = Nitric.Proto.Resource.v1.Resource;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for topic resources.
    ///</Summary>
    public enum TopicPermission
    {
        /// <summary>
        /// Enables pushing new events to the topic.
        /// </summary>
        Publishing
    }

    public class TopicResource : SecureResource<TopicPermission>
    {
        internal TopicResource(string name) : base(name)
        {
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Topic };
            var request = new ResourceDeclareRequest { Resource = resource };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<TopicPermission> permissions)
        {
            var actionMap = new Dictionary<TopicPermission, List<Action>>
            {
                {
                    TopicPermission.Publishing,
                    new List<Action> { Action.TopicEventPublish, Action.TopicList, Action.TopicDetail }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x]))
                .Distinct();
        }

        /// <summary>
        /// Registers a chain of middleware to be called whenever a new event is published to this topic.
        /// </summary>
        /// <param name="handler">The handler to call to process events</param>
        public void Subscribe(params Middleware<EventContext>[] middlewares)
        {
            var subWorker = new Faas(new SubscriptionWorkerOptions { Topic = this.name });

            subWorker.Event(middlewares);
           
            Nitric.RegisterWorker(subWorker);
        }

        /// <summary>
        /// Registers a handler to be called whenever a new event is published to this topic.
        /// </summary>
        /// <param name="handler">The handler to call to process events</param>
        public void Subscribe(Func<EventContext, EventContext> handler)
        {
            var subWorker = new Faas(new SubscriptionWorkerOptions { Topic = this.name });

            subWorker.Event(handler);
            Nitric.RegisterWorker(subWorker);
        }

        /// <summary>
        /// Request specific access to this topic.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public Topic With(params TopicPermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new EventsClient().Topic(this.name);
        }
    }
}
