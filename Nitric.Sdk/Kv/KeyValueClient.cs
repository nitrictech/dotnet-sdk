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
﻿using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using ProtoClient = Nitric.Proto.KeyValue.v1.KeyValue.KeyValueClient;
using Nitric.Proto.KeyValue.v1;
using Nitric.Api.Common;
using System;

namespace Nitric.Api.KeyValue
{
    public class KeyValueClient : AbstractClient
    {
        protected ProtoClient client;
        public System.Type Type { get; private set; }
        public string Collection { get; private set; }

        private KeyValueClient(string collection, System.Type type, ProtoClient client)
        {
            this.Collection = collection;
            this.Type = type;
            this.client = (client == null) ? new ProtoClient(GetChannel()) : client;
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
            var valueStruct = Util.ObjectToStruct(value);
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

        public static Builder NewBuilder() {
            return new Builder();
        }

        public class Builder
        {
            private ProtoClient client;
            private string collection;
            private System.Type type;
            public Builder()
            {
                this.collection = null;
                this.type = null;
                this.client = null;
            }
            public Builder Client(ProtoClient client)
            {
                this.client = client;
                return this;
            }
            public Builder Collection(string collection)
            {
                this.collection = collection;
                return this;
            }
            public Builder Type(System.Type type)
            {
                this.type = type;
                return this;
            }
            public KeyValueClient Build()
            {
                if (string.IsNullOrEmpty(collection))
                {
                    throw new ArgumentNullException("collection");
                }
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }
                return new KeyValueClient(this.collection, this.type, this.client);
            }
            public KeyValueClient Build(string collection)
            {
                return new Builder().Collection(collection).Build();
            }
            public KeyValueClient Build(string collection, System.Type type)
            {
                return new Builder().Collection(collection).Type(type).Build();
            }
        }
    }
}
