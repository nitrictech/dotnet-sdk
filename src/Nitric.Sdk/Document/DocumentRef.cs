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
using System.Linq;
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
    /// <summary>
    /// A reference to a specific document in a collection.
    /// </summary>
    /// <typeparam name="T">The expected type of the document's contents</typeparam>
    public class DocumentRef<T> where T : IDictionary<string, object>, new()
    {
        private readonly DocumentServiceClient documentClient;

        /// <summary>
        /// The unique key of the document.
        /// </summary>
        public readonly Key<T> Key;

        private readonly AbstractCollection<T> collection;

        /// <summary>
        /// Construct a new document reference.
        /// </summary>
        protected DocumentRef()
        {
        }

        internal DocumentRef(
            DocumentServiceClient documentClient,
            AbstractCollection<T> collection,
            string documentId)
        {
            this.documentClient = documentClient;
            this.Key = new Key<T> { Collection = collection, Id = documentId };
            this.collection = collection;
        }

        /// <summary>
        /// Retrieve a document, including its contents
        /// </summary>
        /// <returns>The document</returns>
        /// <exception cref="NitricException"></exception>
        public Document<T> Get()
        {
            var request = new DocumentGetRequest
            {
                Key = this.Key.ToKey()
            };

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

        /// <summary>
        /// Persist the document with updated contents.
        /// </summary>
        /// <param name="value">The contents to store in the document.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public void Set(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var request = new DocumentSetRequest
            {
                Key = this.Key.ToKey(),
                Content = Util.Utils.ObjToStruct(value),
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

        /// <summary>
        /// Delete the document from the collection.
        /// </summary>
        /// <exception cref="NitricException"></exception>
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

        /// <summary>
        /// Create a reference to a sub-collection within this document.
        /// </summary>
        /// <param name="name">The name of the sub-collection.</param>
        /// <returns>The reference to the sub-collection.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public CollectionRef<T> Collection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (Util.Utils.CollectionDepth(this.collection.ToGrpcCollection()) >= Constants.DepthLimit)
            {
                throw new NotSupportedException("Currently sub-collection are only able to be nested " +
                                                Constants.DepthLimit + "deep");
            }

            return new CollectionRef<T>(this.documentClient, name, this.Key);
        }

        /// <summary>
        /// Create a new query builder to find documents in sub-collections of this document.
        /// </summary>
        /// <param name="name">The sub-collection name</param>
        /// <returns>The query builder.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Query<T> Query(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var collectionGroup = new CollectionGroup<T>(
                this.documentClient,
                this.collection.Name,
                this.Key);
            return new Query<T>(this.documentClient, collectionGroup);
        }


        /// <summary>
        /// Utility function to convert a struct document to its generic counterpart
        /// </summary>
        /// <param name="content">The struct to convert.</param>
        /// <returns></returns>
        protected T DocumentToGeneric(Struct content)
        {
            var doc = new T();
            foreach (var kv in content.Fields)
            {
                doc.Add(kv.Key, UnwrapValue(kv.Value));
            }

            return doc;
        }

        private static object UnwrapValue(Value value)
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
                    var unwrappedStruct =
                        value.StructValue.Fields.ToDictionary(kv => kv.Key, kv => UnwrapValue(kv.Value));

                    return unwrappedStruct;
                case Value.KindOneofCase.ListValue:
                    var unwrappedList = value.ListValue.Values.Select(UnwrapValue).ToList();

                    return unwrappedList;
                case Value.KindOneofCase.None:
                default:
                    throw new ArgumentException("Provide proto-value");
            }
        }
    }
}
