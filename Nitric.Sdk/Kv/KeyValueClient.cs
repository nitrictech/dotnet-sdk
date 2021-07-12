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
using GrpcClient = Nitric.Proto.KeyValue.v1.KeyValue.KeyValueClient;
using Nitric.Proto.KeyValue.v1;
using Nitric.Api.Common;
using System;

namespace Nitric.Api.KeyValue
{
    public class KeyValueClient<T> : AbstractClient
    {
        protected GrpcClient client;
        public string Collection { get; private set; }

        private KeyValueClient(string collection, GrpcClient client)
        {
            this.Collection = collection;
            this.client = (client == null) ? new GrpcClient(GetChannel()) : client;
        }
        public void Put(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var valueStruct = Util.ObjToStruct(value);
            var request = new KeyValuePutRequest {
                Collection = Collection,
                Key = key,
                Value = valueStruct
            };
            client.Put(request);
        }
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            var request = new KeyValueGetRequest {
                Collection = Collection,
                Key = key
            };
            var response = client.Get(request);
            var document = response.Value;
            return document.ToString();
        }
        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            var request = new KeyValueDeleteRequest {
                Collection = Collection,
                Key = key
            };
            client.Delete(request);
        }

        public override string ToString()
        {
            return GetType().Name
                    + "[collection=" + Collection
                    + ", type=" + Collection.GetType().Name
                    + "]";
        }

        public static Builder<T> NewBuilder() {
            return new Builder<T>();
        }

        public class Builder<K>
        {
            public GrpcClient client { get; private set; }
            public string collection { get; private set; }

            public Builder<K> Client(GrpcClient client)
            {
                this.client = client;
                return this;
            }
            public Builder<K> Collection(string collection)
            {
                this.collection = collection;
                return this;
            }
            public KeyValueClient<K> Build()
            {
                if (string.IsNullOrEmpty(collection))
                {
                    throw new ArgumentNullException("collection");
                }
                return new KeyValueClient<K>(this.collection, this.client);
            }
        }
    }
}
