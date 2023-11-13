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
using Constants = Nitric.Sdk.Common.Constants;

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// A reference to a collection in the underlying document database.
    /// </summary>
    /// <typeparam name="TDocument">The type of documents stored in the collection.</typeparam>
    public class CollectionRef<TDocument> : AbstractCollection<TDocument>
    {
        internal CollectionRef(DocumentServiceClient documentClient, string name, Key parentKey = null)
            : base(documentClient, name, parentKey)
        {
        }

        /// <summary>
        /// Create a reference to a document within the collection.
        /// </summary>
        /// <param name="documentId">The unique ID of the document.</param>
        /// <returns>A new document reference, which can be used to interact with the document.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DocumentRef<TDocument> Doc(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
            {
                throw new ArgumentNullException(nameof(documentId));
            }

            return new DocumentRef<TDocument>(
                this.DocumentClient,
                this,
                documentId);
        }

        /// <summary>
        /// Returns a reference to all sub-collections with a particular name within this collection.
        ///
        /// E.g. if this is a 'states' collection, the sub-collections could be 'cities'.
        /// </summary>
        /// <param name="name">The sub-collection name</param>
        /// <returns>A reference to all sub-collections with the provided name</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public CollectionGroup<TSubType> Collection<TSubType>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }

            var collectionDepth = this.Depth();
            if (collectionDepth > Constants.DepthLimit)
            {
                throw new NotSupportedException("Currently sub-collections are only able to be nested to a depth of " +
                                                Constants.DepthLimit + ", found depth " + collectionDepth);
            }

            var parentKey = new Key { Collection = this };
            return new CollectionGroup<TSubType>(
                this.DocumentClient,
                name,
                parentKey);
        }
    }
}
