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

using GrpcKey = Nitric.Proto.Document.v1.Key;

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// A fully qualified reference to a specific document in the document service.
    ///
    /// Includes the unique ID of the document and a reference to the collection that contains it.
    /// </summary>
    /// <typeparam name="TDocument">The expected type for the contents of the document.</typeparam>
    public class Key
    {
        /// <summary>
        /// The collection containing this document.
        /// </summary>
        public AbstractCollection Collection { get; set; }

        /// <summary>
        /// The unique ID of this document within its collection.
        /// </summary>
        public string Id { get; set; }

        internal Key()
        {
            // Internal construct to avoid external key creation.
        }

        /// <summary>
        /// Convert this key to a string.
        ///
        /// Useful for logging, should not be used for serialization of the key.
        /// </summary>
        /// <returns>A string with details of this key.</returns>
        public override string ToString()
        {
            return this.GetType().Name + "[collection=" + Collection + ", id=" + Id + "]";
        }

        /// <summary>
        /// Convert this key to its gRPC representation.
        /// </summary>
        /// <returns></returns>
        internal GrpcKey ToKey()
        {
            return new GrpcKey
            {
                Collection = this.Collection.ToGrpcCollection(),
                Id = this.Id ?? "",
            };
        }
    }
}
