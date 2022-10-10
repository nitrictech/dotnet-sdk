using System.Collections.Generic;
using System.Linq;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Queue;
using Action = Nitric.Proto.Resource.v1.Action;
using NitricResource = Nitric.Proto.Resource.v1.Resource;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for queue resources.
    ///</Summary>
    public enum QueuePermission
    {
        /// <summary>
        /// Enables pushing new tasks to the queue.
        /// </summary>
        Sending,
        /// <summary>
        /// Enables pulling and completing tasks on the queue.
        /// </summary>
        Receiving
    }

    class QueueResource : SecureResource<QueuePermission>
    {
        public QueueResource(string name) : base(name)
        {
        }

        public override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Queue };
            var request = new ResourceDeclareRequest { Resource = resource };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<QueuePermission> permissions)
        {
            var actionMap = new Dictionary<QueuePermission, List<Action>>
            {
                {
                    QueuePermission.Sending,
                    new List<Action> { Action.QueueSend, Action.QueueDetail, Action.QueueList }
                },
                {
                    QueuePermission.Receiving,
                    new List<Action> { Action.QueueReceive, Action.QueueDetail, Action.QueueList }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        public Queue.Queue With(params QueuePermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new Queues().Queue(this.name);
        }
    }
}
