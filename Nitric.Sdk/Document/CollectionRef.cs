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
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using Collection = Nitric.Proto.Document.v1.Collection;
using GrpcKey = Nitric.Proto.Document.v1.Key;
using System.Collections.Generic;
namespace Nitric.Api.Document
{
    public class CollectionRef<T> where T : IDictionary<string, object>, new()
    {
        private Collection collection;
        private DocumentServiceClient documentClient;

        internal CollectionRef(DocumentServiceClient documentClient, string name, GrpcKey parentKey = null)
        {
            this.documentClient = documentClient;

            var collection = new Collection()
            {
                Name = name,
            };
            if (parentKey != null)
            {
                collection.Parent = parentKey;
            }
            this.collection = collection;
        }
        public DocumentRef<T> Doc(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
            {
                throw new ArgumentNullException(documentId);
            }
            return new DocumentRef<T>(
                this.documentClient,
                this.collection,
                documentId);
        }
        public Query<T> Query()
        {
            return new Query<T>(this.documentClient, this.collection);
        }
    }
}
