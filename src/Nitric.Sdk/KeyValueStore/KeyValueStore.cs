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

using System.Threading;
using System.Threading.Tasks;
using Nitric.Proto.KvStore.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.KeyValueStore
{
    public class KeyValueStore<T>
    {
        public string Name;
        private readonly KeyValueStoreClient KeyValueClient;

        public KeyValueStore(KeyValueStoreClient client, string name)
        {
            this.KeyValueClient = client;
            this.Name = name;
        }

        /// <summary>
        /// Get a value from the key value store by referencing the key.
        /// </summary>
        /// <param name="key">The unique key that references the value.</param>
        /// <returns>The value that was referenced by the key.</returns>
        public T Get(string key)
        {
            var request = new KvStoreGetValueRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                var resp = KeyValueClient.Client.GetValue(request);

                return Struct.ToJsonSerializable<T>(resp.Value.Content);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Get a value from the key value store by referencing the key.
        /// </summary>
        /// <param name="key">The unique key that references the value.</param>
        /// <returns>The value that was referenced by the key.</returns>
        public async Task<T> GetAsync(string key)
        {
            var request = new KvStoreGetValueRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                var resp = await KeyValueClient.Client.GetValueAsync(request);

                return Struct.ToJsonSerializable<T>(resp.Value.Content);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Set a key value pair in the key value store.
        /// </summary>
        /// <param name="key">A unique key that will reference the value.</param>
        /// <param name="value">The value to store.</param>
        public void Set(string key, T value)
        {
            var request = new KvStoreSetValueRequest
            {
                Content = Struct.FromJsonSerializable(value),
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                KeyValueClient.Client.SetValue(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Set a key value pair in the key value store.
        /// </summary>
        /// <param name="key">A unique key that will reference the value.</param>
        /// <param name="value">The value to store.</param>
        public async Task SetAsync(string key, T value)
        {
            var request = new KvStoreSetValueRequest
            {
                Content = Struct.FromJsonSerializable(value),
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                await KeyValueClient.Client.SetValueAsync(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Delete a value from the key value store.
        /// </summary>
        /// <param name="key">The unique key that references the value.</param>
        public void Delete(string key)
        {
            var request = new KvStoreDeleteKeyRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                KeyValueClient.Client.DeleteKey(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Delete a value from the key value store.
        /// </summary>
        /// <param name="key">The unique key that references the value.</param>
        public async Task DeleteAsync(string key)
        {
            var request = new KvStoreDeleteKeyRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                await KeyValueClient.Client.DeleteKeyAsync(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        public KeyValueKeysResponseStream Keys(string prefix = "")
        {
            var request = new KvStoreScanKeysRequest
            {
                Prefix = prefix,
                Store = new Store
                {
                    Name = this.Name,
                },
            };

            try
            {
                var resp = KeyValueClient.Client.ScanKeys(request);

                return new KeyValueKeysResponseStream(resp.ResponseStream);

            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }

    public class KeyValueKeysResponseStream
    {
        private Grpc.Core.IAsyncStreamReader<KvStoreScanKeysResponse> stream;

        internal KeyValueKeysResponseStream(Grpc.Core.IAsyncStreamReader<KvStoreScanKeysResponse> stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Advances the reader to the next element in the sequence, returning the result asynchronously
        /// </summary>
        /// <returns>Task containing the result of the operation: true if the reader was successfully advanced to the next element; false if the reader has passed the end of the sequence.</returns>
        public Task<bool> MoveNext()
        {
            return stream.MoveNext(CancellationToken.None);
        }

        /// <summary>
        /// Advances the reader to the next element in the sequence, returning the result asynchronously
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel the operation.</param>
        /// <returns>Task containing the result of the operation: true if the reader was successfully advanced to the next element; false if the reader has passed the end of the sequence.</returns>
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return stream.MoveNext(cancellationToken);
        }


        /// <summary>
        /// Gets the current key in the iteration.
        /// </summary>
        public string Current { get => stream.Current.Key; }
    }
}

