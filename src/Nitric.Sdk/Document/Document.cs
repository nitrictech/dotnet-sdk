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

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// Represents a point-in-time version of a document, including the contents.
    /// </summary>
    /// <typeparam name="T">The type of the contents of this document</typeparam>
    public class Document<T> where T : IDictionary<string, object>, new()
    {
        /// <summary>
        /// The reference to the document in the document database.
        /// </summary>
        public DocumentRef<T> Ref { get; private set; }
        /// <summary>
        /// The document contents.
        /// </summary>
        public T Content { get; private set; }

        internal Document(DocumentRef<T> documentRef, T content)
        {
            this.Ref = documentRef;
            this.Content = content;
        }
    }
}
