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
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using ProtoBlob = Nitric.Proto.Storage.v1.Blob;


namespace Nitric.Sdk.Storage
{
    /// <summary>
    /// A reference to a bucket in the storage service.
    /// </summary>
    public class Bucket
    {
        internal readonly StorageClient Storage;

        /// <summary>
        /// The name of the bucket.
        /// </summary>
        public string Name { get; private set; }

        internal Bucket(StorageClient storage, string name)
        {
            this.Storage = storage;
            this.Name = name;
        }

        /// <summary>
        /// Create a reference to a blob in the bucket.
        /// </summary>
        /// <param name="key">The blobs name/path</param>
        /// <returns>The blob reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Blob File(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return new Blob(this, key);
        }

        /// <summary>
        /// Get a list of blobs in a bucket.
        /// </summary>
        /// <returns>All the files in the bucket as Nitric blob references.</returns>
        public List<Blob> Files()
        {
            var request = new StorageListBlobsRequest
            {
                BucketName = this.Name,
            };

            var resp = this.Storage.Client.ListBlobs(request);

            List<Blob> files = new List<Blob>();

            foreach (ProtoBlob file in resp.Blobs)
            {
                files.Add(new Blob(this, file.Key));
            }

            return files;
        }

        /// <summary>
        /// Registers handlers to be called whenever a blob triggers an event in the bucket
        /// </summary>
        /// <param name="blobEventType">The type of events that should trigger events, Write or Delete</param>
        /// <param name="keyPrefixFilter">The prefix of blob names that should trigger events</param>
        /// <param name="middlewares">The handlers to call to process notification events</param>
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
        /// Registers a handler to be called whenever a blob triggers an event in the bucket
        /// </summary>
        /// <param name="blobEventType">The type of events that should trigger events, Write or Delete</param>
        /// <param name="keyPrefixFilter">The prefix of blob names that should trigger events</param>
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
    }
}
