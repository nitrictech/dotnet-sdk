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
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.KvStore.v1.KvStore.KvStoreClient;

namespace Nitric.Sdk.KeyValueStore
{
    public class KeyValueStoreClient
    {
        /// <summary>
        /// Construct a new key value client.
        /// </summary>
        /// <param name="client">Optional gRPC client to reuse.</param>
        internal readonly GrpcClient Client;

        public KeyValueStoreClient(GrpcClient client = null)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to the key value store from the key value service.
        /// </summary>
        /// <param name="name">The key value store name</param>
        /// <typeparam name="T">The expected type for values in the key value store.</typeparam>
        /// <returns>The collection reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public KeyValueStore<T> KV<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new KeyValueStore<T>(this, name);
        }
    }
}

