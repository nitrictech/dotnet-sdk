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
    /// Available permissions for bucket resources.
    ///</Summary>
    public enum CollectionPermission
    {
        /// <summary>
        /// Enables listing and reading files in the bucket.
        /// </summary>
        Reading,
        /// <summary>
        /// Enables adding or updating files in the bucket.
        /// </summary>
        Writing,
        /// <summary>
        /// Enables deleting files from the bucket.
        /// </summary>
        Deleting
    }

    public class CollectionResource : SecureResource<CollectionPermission>
    {
        internal CollectionResource(string name) : base(name)
        {
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Collection };
            var request = new ResourceDeclareRequest { Resource = resource };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<CollectionPermission> permissions)
        {
            var actionMap = new Dictionary<CollectionPermission, List<Action>>
            {
                {
                    CollectionPermission.Reading,
                    new List<Action> { Action.CollectionList, Action.CollectionDocumentRead, Action.CollectionQuery }
                },
                {
                    CollectionPermission.Writing,
                    new List<Action> { Action.CollectionList, Action.CollectionDocumentWrite }
                },
                {
                    CollectionPermission.Deleting,
                    new List<Action> { Action.CollectionList, Action.CollectionDocumentDelete }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        public Collection<Dictionary<string, object>> With(params CollectionPermission[] permissions)
        {
            this.RegisterPolicy(permissions);
            return new DocumentsClient().Collection<Dictionary<string, object>>(this.name);
        }
    }
}
