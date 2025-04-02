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
using NitricSecret = Nitric.Sdk.Secret.Secret;
using Action = Nitric.Proto.Resources.v1.Action;
using GrpcClient = Nitric.Proto.Secrets.v1.SecretManager.SecretManagerClient;
using Nitric.Sdk.Common;

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
        Put,
        /// <summary>
        /// Enables accessing secrets from the secret store.
        /// </summary>
        Access
    }

    public class SecretResource : SecureResource<SecretPermission>
    {
        internal readonly GrpcClient Client;

        internal SecretResource(string name, GrpcClient client = null) : base(name, ResourceType.Secret)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<SecretPermission> permissions)
        {
            var actionMap = new Dictionary<SecretPermission, List<Action>>
            {
                {
                    SecretPermission.Put,
                    new List<Action> { Action.SecretPut }
                },
                {
                    SecretPermission.Access,
                    new List<Action> { Action.SecretAccess }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        public NitricSecret Allow(SecretPermission permission, params SecretPermission[] permissions)
        {
            var allPerms = new List<SecretPermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);

            return new NitricSecret(this.Name, this.Client);
        }
    }
}
