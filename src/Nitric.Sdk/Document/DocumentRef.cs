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
using System.Linq;
using Google.Protobuf;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using GrpcKey = Nitric.Proto.Document.v1.Key;
using Nitric.Proto.Document.v1;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using RpcException = Grpc.Core.RpcException;
using NitricException = Nitric.Sdk.Common.NitricException;
using Constants = Nitric.Sdk.Common.Constants;

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// A reference to a specific document in a collection.
    /// </summary>
    /// <typeparam name="TDocument">The expected type of the document's contents</typeparam>
    public class DocumentRef<TDocument>
    {
        private readonly DocumentServiceClient documentClient;

        /// <summary>
        /// The unique key of the document.
        /// </summary>
        public readonly Key Key;

        private readonly AbstractCollection collection;

        /// <summary>
        /// Construct a new document reference.
        /// </summary>
        protected DocumentRef()
        {
        }

        internal DocumentRef(
            DocumentServiceClient documentClient,
            AbstractCollection collection,
            string documentId)
        {
            this.documentClient = documentClient;
            this.Key = new Key { Collection = collection, Id = documentId };
            this.collection = collection;
        }

        /// <summary>
        /// Retrieve a document, including its contents
        /// </summary>
        /// <returns>The document</returns>
        /// <exception cref="NitricException"></exception>
        public Document<TDocument> Get()
        {
            var request = new DocumentGetRequest
            {
                Key = this.Key.ToKey()
            };

            try
            {
                var response = this.documentClient.Get(request);
                return new Document<TDocument>(
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
        public void Set(TDocument value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var json = JsonConvert.SerializeObject(value);
            var content = JsonParser.Default.Parse<Struct>(json);

            var request = new DocumentSetRequest
            {
                Key = this.Key.ToKey(),
                Content = content
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
        public CollectionRef<TSubType> Collection<TSubType>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var collectionDepth = this.Key.Collection.Depth();
            if (collectionDepth > Constants.DepthLimit)
            {
                throw new NotSupportedException("Currently sub-collections are only able to be nested to a depth of " +
                                                Constants.DepthLimit + ", found depth " + collectionDepth);
            }

            return new CollectionRef<TSubType>(this.documentClient, name, this.Key);
        }

        /// <summary>
        /// Create a new query builder to find documents in sub-collections of this document.
        /// </summary>
        /// <param name="name">The sub-collection name</param>
        /// <returns>The query builder.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Query<TDocument> Query(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var collectionGroup = new CollectionGroup<TDocument>(
                this.documentClient,
                this.collection.Name,
                this.Key);
            return new Query<TDocument>(this.documentClient, collectionGroup);
        }

        /// <summary>
        /// Utility function to convert a struct document to its generic counterpart
        /// </summary>
        /// <param name="content">The struct to convert.</param>
        /// <returns></returns>
        protected TDocument DocumentToGeneric(Struct content)
        {
            var doc = content.Fields.ToDictionary(
                kv => kv.Key, kv => UnwrapValue(kv.Value));

            var dictInJson = JsonConvert.SerializeObject(doc, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
            });

            return JsonConvert.DeserializeObject<TDocument>(dictInJson);
        }

        internal static object UnwrapValue(Value value)
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
