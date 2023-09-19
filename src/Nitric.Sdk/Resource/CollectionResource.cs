using System.Collections.Generic;
using System.Linq;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Document;
using Action = Nitric.Proto.Resource.v1.Action;
using NitricResource = Nitric.Proto.Resource.v1.Resource;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for collection resources.
    ///</Summary>
    public enum CollectionPermission
    {
        /// <summary>
        /// Enables writing documents to the collection,.
        /// </summary>
        Writing,
        /// <summary>
        /// Enables reading documents from the collection.
        /// </summary>
        Reading,
        /// <summary>
        /// Enables deleting documents from the collection.
        /// </summary>
        Deleting,
    }

    public class CollectionResource<T> : SecureResource<CollectionPermission>
    {
        internal CollectionResource(string name) : base(name, ResourceType.Collection)
        {
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.Name, Type = ResourceType.Collection };
            var request = new ResourceDeclareRequest { Resource = resource };
            client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<CollectionPermission> permissions)
        {
            var actionMap = new Dictionary<CollectionPermission, List<Action>>
            {
                {
                    CollectionPermission.Writing,
                    new List<Action> { Action.CollectionDocumentWrite }
                },
                {
                    CollectionPermission.Reading,
                    new List<Action> { Action.CollectionList, Action.CollectionQuery, Action.CollectionDocumentRead }
                },
                {
                    CollectionPermission.Deleting,
                    new List<Action> { Action.CollectionDocumentDelete }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x]))
                .Distinct();
        }

        public CollectionRef<T> With(params CollectionPermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new DocumentsClient().Collection<T>(this.Name);
        }
    }
}
