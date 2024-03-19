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
using Nitric.Sdk.Service;
using Nitric.Sdk.Storage;
using Nitric.Sdk.Worker;
using Nitric.Proto.Storage.v1;
using Action = Nitric.Proto.Resources.v1.Action;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;
using System.Runtime.InteropServices;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for bucket resources.
    ///</Summary>
    public enum BucketPermission
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

    public class BucketResource : SecureResource<BucketPermission>
    {
        internal BucketResource(string name) : base(name, ResourceType.Bucket)
        {
        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<BucketPermission> permissions)
        {
            var actionMap = new Dictionary<BucketPermission, List<Action>>
            {
                {
                    BucketPermission.Reading,
                    new List<Action> { Action.BucketFileList, Action.BucketFileGet }
                },
                {
                    BucketPermission.Writing,
                    new List<Action> { Action.BucketFilePut }
                },
                {
                    BucketPermission.Deleting,
                    new List<Action> { Action.BucketFileDelete }
                }
            };
            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        /// <summary>
        /// Registers handlers to be called whenever a file triggers an event in the bucket
        /// </summary>
        /// <param name="blobEventType">The type of events that should trigger events, Write or Delete</param>
        /// <param name="keyPrefixFilter">The prefix of file names that should trigger events</param>
        /// <param name="middleware">The handlers to call to process notification events</param>
        public void On(
            Service.BlobEventType blobEventType,
            string keyPrefixFilter,
            params Middleware<BlobEventContext>[] middlewares)
        {
            var request = new RegistrationRequest
            {
                BucketName = this.Name,
                KeyPrefixFilter = keyPrefixFilter,
                BlobEventType = blobEventType.ToGrpc(),
            };
            var notificationWorker = new BlobEventWorker(request, middlewares);

            Nitric.RegisterWorker(notificationWorker);
        }

        /// <summary>
        /// Registers a handler to be called whenever a file triggers an event in the bucket
        /// </summary>
        /// <param name="blobEventType">The type of events that should trigger events, Write or Delete</param>
        /// <param name="keyPrefixFilter">The prefix of file names that should trigger events</param>
        /// <param name="handler">The handler to call to process notification events</param>
        public void On(
            Service.BlobEventType blobEventType,
            string keyPrefixFilter,
            Func<BlobEventContext, BlobEventContext> handler)
        {
            var request = new RegistrationRequest
            {
                BucketName = this.Name,
                KeyPrefixFilter = keyPrefixFilter,
                BlobEventType = blobEventType.ToGrpc(),
            };
            var notificationWorker = new BlobEventWorker(request, handler);

            Nitric.RegisterWorker(notificationWorker);
        }

        /// <summary>
        /// Request specific access to this bucket.
        /// </summary>
        /// <param name="permissions">The permissions that the function has to access the bucket.</param>
        /// <returns>A reference to the bucket.</returns>
        public Bucket Allow(BucketPermission permission, params BucketPermission[] permissions)
        {
            var allPerms = new List<BucketPermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);
            return new StorageClient().Bucket(this.Name);
        }
    }
}
