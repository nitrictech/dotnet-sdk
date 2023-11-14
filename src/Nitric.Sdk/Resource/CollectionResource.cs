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

        public CollectionRef<T> With(CollectionPermission permission, params CollectionPermission[] permissions)
        {
            var allPerms = new List<CollectionPermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);
            return new DocumentsClient().Collection<T>(this.Name);
        }
    }
}
