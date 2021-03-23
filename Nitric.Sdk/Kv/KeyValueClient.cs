using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using ProtoClient = Nitric.Proto.KeyValue.v1.KeyValue.KeyValueClient;
using Nitric.Proto.KeyValue.v1;
using Nitric.Api.Common;
//using CommonEvent = Nitric.Api.Common.Event;

namespace Nitric.Api.KeyValue
{
    class KeyValueClient : AbstractClient
    {
        protected ProtoClient client;

        public KeyValueClient()
        {
            client = new ProtoClient(GetChannel());
        }
        public void Put(string collection, string key, Dictionary<string, string> value)
        {
            var valueStruct = Util.ObjectToStruct(value);
            var request = new KeyValuePutRequest { Collection = collection, Key = key, Value = valueStruct };
            var response = client.Put(request);
        }
        public string Get(string collection, string key)
        {
            var request = new KeyValueGetRequest { Collection = collection, Key = key };
            var response = client.Get(request);
            var document = response.Value;
            return document.ToString();
        }
        public void Delete(string collection, string key)
        {
            var request = new KeyValueDeleteRequest { Collection = collection, Key = key };
            var response = client.Delete(request);
        }
    }
}
