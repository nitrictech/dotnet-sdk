using System.Collections.Generic;
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
        public string Collection { get; private set; }

        private KeyValueClient(string collection)
        {
            this.Collection = collection;
            client = new ProtoClient(GetChannel());
        }
        public void Put(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key parameter required");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value parameter required");
            }
            var valueStruct = Util.ObjectToStruct(value);
            var request = new KeyValuePutRequest { Collection = Collection, Key = key, Value = valueStruct };
            var response = client.Put(request);
        }
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key parameter required");
            }
            var request = new KeyValueGetRequest { Collection = Collection, Key = key };
            var response = client.Get(request);
            var document = response.Value;
            return document.ToString();
        }
        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key parameter required");
            }
            var request = new KeyValueDeleteRequest { Collection = Collection, Key = key };
            var response = client.Delete(request);
        }

        public class Builder
        {
            private string collection;

            public Builder()
            {
                this.collection = null;
            }
            public Builder Collection(string collection)
            {
                this.collection = collection;
                return this;
            }
            public KeyValueClient Build()
            {
                return new KeyValueClient(this.collection);
            }
            public KeyValueClient Build(string collection)
            {
                return new KeyValueClient(collection);
            }
        }
    }
}
