using System;
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.KeyValue.v1.KeyValue.KeyValueClient;

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
        public KeyValueStore<T> Store<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new KeyValueStore<T>(this, name);
        }
    }
}

