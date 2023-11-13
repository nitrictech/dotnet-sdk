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

namespace Nitric.Sdk.Document
{
    public abstract class AbstractCollection
    {
        internal readonly DocumentServiceClient DocumentClient;

        /// <summary>
        /// The name of the collection
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// A reference to the document this collection sits below.
        ///
        /// If set, this is a sub-collection.
        /// </summary>
        public readonly Key ParentKey;

        /// <summary>
        /// Construct a new collection
        /// </summary>
        /// <param name="documentClient">The client reference to use for operations on this collection</param>
        /// <param name="name">The name of the collection</param>
        /// <param name="parentKey">An optional parent key</param>
        /// <exception cref="ArgumentNullException">Throws if a required parameter is missing</exception>
        protected AbstractCollection(DocumentServiceClient documentClient, string name, Key parentKey)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            this.ParentKey = parentKey;
            this.DocumentClient = documentClient;
        }

        internal Collection ToGrpcCollection()
        {
            var collection = new Collection()
            {
                Name = this.Name,
            };
            if (this.ParentKey != null)
            {
                collection.Parent = this.ParentKey.ToKey();
            }

            return collection;
        }

        /// <summary>
        /// Determines the depth of this collection.
        ///
        /// If the collection has no parent collections the depth will be 1. The depth is increased by 1 for each parent.
        /// </summary>
        /// <returns></returns>
        public int Depth()
        {
            var parentDepth = (this.ParentKey != null) ? this.ParentKey.Collection.Depth() : 0;
            return 1 + parentDepth;
        }
    }

    /// <summary>
    /// Document collection base class
    /// </summary>
    /// <typeparam name="TDocument">The type for documents in this collection</typeparam>
    public abstract class AbstractCollection<TDocument> : AbstractCollection
    {

        public AbstractCollection(DocumentServiceClient documentClient, string name, Key parentKey) : base(documentClient, name, parentKey) {

        }

        /// <summary>
        /// Construct a query to find documents in the collection
        /// </summary>
        /// <returns>A new query builder</returns>
        public Query<TDocument> Query()
        {
            return new Query<TDocument>(this.DocumentClient, this);
        }

    }
}
