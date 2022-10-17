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
using GrpcClient = Nitric.Proto.Storage.v1.StorageService.StorageServiceClient;
using Nitric.Sdk.Common.Util;

namespace Nitric.Sdk.Storage
{
    /// <summary>
    /// A storage client.
    /// </summary>
    public class Storage
    {
        internal readonly GrpcClient Client;

        /// <summary>
        /// Create a new storage client.
        /// </summary>
        /// <param name="client">Optional internal gRPC client to reuse.</param>
        public Storage(GrpcClient client = null)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to a bucket in the storage service.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <returns>The new bucket reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Bucket Bucket(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            return new Bucket(this, bucketName);
        }
    }
}
