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
using Nitric.Proto.Resources.v1;
using Nitric.Sdk.KeyValueStore;
using Action = Nitric.Proto.Resources.v1.Action;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;
using ResourceType = Nitric.Proto.Resources.v1.ResourceType;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for collection resources.
    ///</Summary>
    public enum KeyValueStorePermission
    {
        /// <summary>
        /// Enables setting values to the key value store,.
        /// </summary>
        Setting,
        /// <summary>
        /// Enables getting values from the key value store.
        /// </summary>
        Getting,
        /// <summary>
        /// Enables deleting values from the key value store.
        /// </summary>
        Deleting,
    }

    public class KeyValueStoreResource<TValue> : SecureResource<KeyValueStorePermission>
    {
        internal KeyValueStoreResource(string name) : base(name, ResourceType.KeyValueStore)
        {
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.Name, Type = ResourceType.KeyValueStore };
            var request = new ResourceDeclareRequest { Id = resource };
            client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<KeyValueStorePermission> permissions)
        {
            var actionMap = new Dictionary<KeyValueStorePermission, List<Action>>
            {
                {
                    KeyValueStorePermission.Setting,
                    new List<Action> { Action.KeyValueStoreWrite }
                },
                {
                    KeyValueStorePermission.Getting,
                    new List<Action> { Action.KeyValueStoreRead }
                },
                {
                    KeyValueStorePermission.Deleting,
                    new List<Action> { Action.KeyValueStoreDelete }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x]))
                .Distinct();
        }

        public KeyValueStore<TValue> With(KeyValueStorePermission permission, params KeyValueStorePermission[] permissions)
        {
            var allPerms = new List<KeyValueStorePermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);
            return new KeyValueStoreClient().Store<TValue>(this.Name);
        }
    }
}
