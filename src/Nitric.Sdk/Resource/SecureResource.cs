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
using Nitric.Proto.Resource.v1;
using Action = Nitric.Proto.Resource.v1.Action;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;

namespace Nitric.Sdk.Resource
{
    /// <summary>
    /// Represents a resource that requires permissions to enable access.
    /// </summary>
    /// <typeparam name="T">The type of permissions that secure this resource.</typeparam>
    public abstract class SecureResource<T> : BaseResource
    {
        /// <summary>
        /// Construct a new secure resource.
        /// </summary>
        /// <param name="name">The name of the secure resource</param>
        /// <param name="type">The type of the secure resource</param>
        protected SecureResource(string name, ResourceType type) : base(name, type)
        {
        }

        /// <summary>
        /// Converts a list of permission enum values to the set of action that provide that permission.
        /// </summary>
        /// <returns>A list of resource actions</returns>
        protected abstract IEnumerable<Action> PermissionsToActions(IEnumerable<T> permissions);

        /// <summary>
        /// Register a policy to provide the list of supplied permissions to this resource.
        /// </summary>
        /// <param name="permissions">The permissions to include with the resource policy.</param>
        protected void RegisterPolicy(IEnumerable<T> permissions)
        {
            var policyResource = new Proto.Resource.v1.Resource { Type = ResourceType.Policy };
            var defaultPrincipal = new Proto.Resource.v1.Resource { Type = ResourceType.Function };

            var actions = this.PermissionsToActions(permissions);

            var policy = new PolicyResource { Principals = { defaultPrincipal }, Actions = { actions } };
            policy.Resources.Add(this.AsProtoResource());

            var request = new ResourceDeclareRequest { Resource = policyResource, Policy = policy };
            BaseResource.client.Declare(request);
        }
    }
}
