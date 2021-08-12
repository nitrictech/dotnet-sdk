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
using System.Collections.Generic;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using GrpcKey = Nitric.Proto.Document.v1.Key;
using Nitric.Proto.Document.v1;
using Util = Nitric.Api.Common.Util;
using Google.Protobuf.WellKnownTypes;
using RpcException = Grpc.Core.RpcException;
using NitricException = Nitric.Api.Common.NitricException;

namespace Nitric.Api.Document
{
    public class DocumentRef<T> where T : IDictionary<string, object>, new()
    {
        const int DEPTH_LIMIT = 1;

        private readonly DocumentServiceClient documentClient;
        public readonly Key<T> Key;
        private readonly CollectionRef<T> collection;

        protected DocumentRef() { }
        internal DocumentRef(
            DocumentServiceClient documentClient,
            CollectionRef<T> collection,
            string documentId)
        {
            this.documentClient = documentClient;
            this.Key = new Key<T>(collection, documentId);
            this.collection = collection;
        }

        public Document<T> Get()
        {
            DocumentGetRequest request = new DocumentGetRequest();
            request.Key = this.Key.ToKey();

            try
            {
                var response = this.documentClient.Get(request);
                return new Document<T>(
                    this,
                    DocumentToGeneric(response.Document.Content)
                );
            }
            catch (RpcException re)
            {
                throw NitricException.Exceptions[re.StatusCode](re.Message);
            }
        }
        public void Set(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var request = new DocumentSetRequest
            {
                Key = this.Key.ToKey(),
                Content = Util.ObjToStruct(value),
            };
            try
            {
                this.documentClient.Set(request);
            }
            catch (RpcException re)
            {
                throw NitricException.Exceptions[re.StatusCode](re.Message);
            }
        }

        public void Delete()
        {
            var request = new DocumentDeleteRequest
            {
                Key = this.Key.ToKey(),
            };
            try
            {
                this.documentClient.Delete(request);
            }
            catch (RpcException re)
            {
                throw NitricException.Exceptions[re.StatusCode](re.Message);
            }
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
                throw new ArgumentNullException("name");
            }
            if (Depth(0, this.collection.ToGrpcCollection()) >= DEPTH_LIMIT)
            {
                throw new NotSupportedException("Currently subcollection are only able to be nested " + DEPTH_LIMIT + "deep");
            }
            return new CollectionRef<T>(this.documentClient, name, this.Key);
        }

        //Utility function to convert a struct document to its generic counterpart
        protected T DocumentToGeneric(Struct content)
        {
            T doc = new T();
            foreach (var kv in content.Fields)
            {
                doc.Add(kv.Key, UnwrapValue(kv.Value));
            }
            return doc;
        }
        private object UnwrapValue(Value value)
        {
            switch (value.KindCase)
            {
                case Value.KindOneofCase.StringValue:
                    return value.StringValue;
                case Value.KindOneofCase.BoolValue:
                    return value.BoolValue;
                case Value.KindOneofCase.NumberValue:
                    return value.NumberValue;
                case Value.KindOneofCase.NullValue:
                    return null;
                case Value.KindOneofCase.StructValue:
                    Dictionary<string, object> unwrappedStruct = new Dictionary<string, object>();
                    foreach (var kv in value.StructValue.Fields)
                    {
                        unwrappedStruct.Add(kv.Key, UnwrapValue(kv.Value));
                    }
                    return unwrappedStruct;
                case Value.KindOneofCase.ListValue:
                    List<object> unwrappedList = new List<object>();
                    foreach (Value v in value.ListValue.Values)
                    {
                        unwrappedList.Add(UnwrapValue(v));
                    }
                    return unwrappedList;
                default:
                    throw new ArgumentException("Provide proto-value");
            }
        }
    }
}