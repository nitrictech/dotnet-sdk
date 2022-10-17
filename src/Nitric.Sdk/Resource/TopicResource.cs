using System;
using System.Collections.Generic;
using System.Linq;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Event;
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

    class TopicResource : SecureResource<TopicPermission>
    {
        internal TopicResource(string name) : base(name)
        {
        }

        public override BaseResource Register()
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

        // public void subscribe(EventHandler<> mw)
        // {
        //
        // }

        public Topic With(params TopicPermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new EventsClient().Topic(this.name);
        }
    }
}
