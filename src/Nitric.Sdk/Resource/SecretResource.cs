using System.Collections.Generic;
using System.Linq;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Secret;
using Action = Nitric.Proto.Resource.v1.Action;
using NitricResource = Nitric.Proto.Resource.v1.Resource;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for secret resources.
    ///</Summary>
    public enum SecretPermission
    {
        /// <summary>
        /// Enables putting secrets to the secret store.
        /// </summary>
        Putting,
        /// <summary>
        /// Enables accessing secrets from the secret store.
        /// </summary>
        Accessing
    }

    public class SecretResource : SecureResource<SecretPermission>
    {
        internal SecretResource(string name) : base(name, ResourceType.Secret)
        {
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.Name, Type = ResourceType.Secret };
            var request = new ResourceDeclareRequest { Resource = resource };
            client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<SecretPermission> permissions)
        {
            var actionMap = new Dictionary<SecretPermission, List<Action>>
            {
                {
                    SecretPermission.Putting,
                    new List<Action> { Action.SecretPut }
                },
                {
                    SecretPermission.Accessing,
                    new List<Action> { Action.SecretAccess }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        public Secret.Secret With(params SecretPermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new SecretsClient().Secret(this.Name);
        }
    }
}
