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

        public Secret.Secret With(SecretPermission permission, params SecretPermission[] permissions)
        {
            var allPerms = new List<SecretPermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);
            return new SecretsClient().Secret(this.Name);
        }
    }
}
