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

using System.Collections.Generic;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// Represents a group of collections, used to search for documents in across multiple collections.
    ///
    /// This is typically used to search all sub-collections below docs in a single parent collections.
    ///
    /// E.g. search all cities in all states. The group of sub-collections are expected to have the same name (e.g. "cities")
    /// </summary>
    /// <typeparam name="T">The type of documents contained in the group of collections</typeparam>
    public class CollectionGroup<T> : AbstractCollection<T>
    {
        /// <summary>
        /// Construct a new collection group
        /// </summary>
        /// <param name="documentClient">The document client used to interact with this group</param>
        /// <param name="name">A name of the collections</param>
        /// <param name="parentKey"></param>
        public CollectionGroup(DocumentServiceClient documentClient, string name, Key<T> parentKey)
            : base(documentClient, name, parentKey)
        {
        }
    }
}
