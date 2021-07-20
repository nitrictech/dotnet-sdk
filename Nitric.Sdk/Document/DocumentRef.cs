using System;
using System.Collections.Generic;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using GrpcKey = Nitric.Proto.Document.v1.Key;
using Nitric.Proto.Document.v1;
using Util = Nitric.Api.Common.Util;
using Google.Protobuf.WellKnownTypes;
namespace Nitric.Api.Document
{
    public class DocumentRef<T> where T : IDictionary<string, object>, new()
    {
        const int DEPTH_LIMIT = 1;

        private readonly DocumentServiceClient documentClient;
        private readonly GrpcKey key;
        private readonly Collection collection;

        internal DocumentRef(
            DocumentServiceClient documentClient,
            Collection collection,
            string documentId)
        {
            this.documentClient = documentClient;
            this.key = new Key(collection, documentId).ToKey();
            this.collection = collection;
        }

        public T Get()
        {
            DocumentGetRequest request = new DocumentGetRequest();
            request.Key = this.key;

            DocumentGetResponse response = null;
            response = this.documentClient.Get(request);

            return DocumentToGeneric(response.Document.Content);
        }
        public void Set(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Provide non-null value");
            }
            var request = new DocumentSetRequest
            {
                Key = this.key,
                Content = Util.ObjToStruct(value),
            };
            this.documentClient.Set(request);
        }

        public void Delete()
        {
            var request = new DocumentDeleteRequest
            {
                Key = this.key,
            };
            this.documentClient.Delete(request);
        }

        private int Depth(int depth, Collection collection)
        {
            return (collection.Parent != null) ?
                Depth(depth + 1, collection.Parent.Collection) : depth;
        }

        public CollectionRef<T> Collection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            if (Depth(0, this.collection) >= DEPTH_LIMIT)
            {
                throw new NotSupportedException("Currently subcollection are only able to be nested " + DEPTH_LIMIT + "deep");
            }
            return new CollectionRef<T>(this.documentClient, name, this.key);
        }

        //Utility function to convert a struct document to its generic counterpart
        private T DocumentToGeneric(Struct content)
        {
            T doc = new T();
            foreach (var kv in content.Fields)
            {
                switch (kv.Value.KindCase)
                {
                    case Value.KindOneofCase.StringValue:
                        doc.Add(kv.Key, kv.Value.StringValue);
                        break;
                    case Value.KindOneofCase.BoolValue:
                        doc.Add(kv.Key, kv.Value.BoolValue);
                        break;
                    case Value.KindOneofCase.ListValue:
                        doc.Add(kv.Key, kv.Value.ListValue);
                        break;
                    case Value.KindOneofCase.NumberValue:
                        doc.Add(kv.Key, kv.Value.NumberValue);
                        break;
                    case Value.KindOneofCase.StructValue:
                        doc.Add(kv.Key, kv.Value.StructValue);
                        break;
                    case Value.KindOneofCase.NullValue:
                        doc.Add(kv.Key, kv.Value.NullValue);
                        break;
                }
            }
            return doc;
        }
    }
}