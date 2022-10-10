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
using Util = Nitric.Sdk.Common.Util;
using Google.Protobuf.WellKnownTypes;
using RpcException = Grpc.Core.RpcException;
using NitricException = Nitric.Sdk.Common.NitricException;
using Constants = Nitric.Sdk.Common.Constants;

namespace Nitric.Sdk.Document
{
    public class DocumentRef<T> where T : IDictionary<string, object>, new()
    {
        private readonly DocumentServiceClient documentClient;
        public readonly Key<T> Key;
        private readonly AbstractCollection<T> collection;

        protected DocumentRef() { }
        internal DocumentRef(
            DocumentServiceClient documentClient,
            AbstractCollection<T> collection,
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
                throw NitricException.FromRpcException(re);
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
                throw NitricException.FromRpcException(re);
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
                throw NitricException.FromRpcException(re);
            }
        }

        public CollectionRef<T> Collection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (Util.CollectionDepth(0, this.collection.ToGrpcCollection()) >= Constants.DEPTH_LIMIT)
            {
                throw new NotSupportedException("Currently subcollection are only able to be nested " + Constants.DEPTH_LIMIT + "deep");
            }
            return new CollectionRef<T>(this.documentClient, name, this.Key);
        }

        public Query<T> Query(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            var collectionGroup = new CollectionGroup<T>(
                this.documentClient,
                this.collection.Name,
                this.Key);
            return new Query<T>(this.documentClient, collectionGroup);
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
