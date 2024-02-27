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
using Nitric.Sdk.Queue;
using Action = Nitric.Proto.Resources.v1.Action;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;

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
        Enqueue,
        /// <summary>
        /// Enables pulling and completing tasks on the queue.
        /// </summary>
        Dequeue
    }

    public class QueueResource<T> : SecureResource<QueuePermission>
    {
        internal QueueResource(string name) : base(name, ResourceType.Queue)
        {
        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<QueuePermission> permissions)
        {
            var actionMap = new Dictionary<QueuePermission, List<Action>>
            {
                {
                    QueuePermission.Enqueue,
                    new List<Action> { Action.QueueEnqueue }
                },
                {
                    QueuePermission.Dequeue,
                    new List<Action> { Action.QueueDequeue }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        /// <summary>
        /// Request specific access to this queue.
        /// </summary>
        /// <param name="permissions">The permissions that the function has to access the queue.</param>
        /// <returns>A reference to the queue.</returns>
        public Queue.Queue<T> With(QueuePermission permission, params QueuePermission[] permissions)
        {
            var allPerms = new List<QueuePermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);
            return new QueuesClient().Queue<T>(this.Name);
        }
    }
}
