using System.Threading.Tasks;
using Nitric.Proto.KeyValue.v1;
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
            var request = new KeyValueGetRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                var resp = KeyValueClient.Client.Get(request);

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
            var request = new KeyValueGetRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                var resp = await KeyValueClient.Client.GetAsync(request);

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
            var request = new KeyValueSetRequest
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
                KeyValueClient.Client.Set(request);
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
            var request = new KeyValueSetRequest
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
                await KeyValueClient.Client.SetAsync(request);
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
            var request = new KeyValueDeleteRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                KeyValueClient.Client.Delete(request);
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
            var request = new KeyValueDeleteRequest
            {
                Ref = new ValueRef
                {
                    Store = Name,
                    Key = key,
                }
            };

            try
            {
                await KeyValueClient.Client.DeleteAsync(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}

