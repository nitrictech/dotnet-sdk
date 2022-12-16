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
using Nitric.Sdk.Common;
using Nitric.Sdk.Common.Util;

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// A documents service reference.
    /// </summary>
    public class DocumentsClient
    {
        private readonly DocumentServiceClient documentClient;

        /// <summary>
        /// Construct a new documents client.
        /// </summary>
        /// <param name="client">Optional gRPC client to reuse.</param>
        public DocumentsClient(DocumentServiceClient client = null)
        {
            this.documentClient = client ?? new DocumentServiceClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to collection from the documents service.
        /// </summary>
        /// <param name="name">The collection name</param>
        /// <typeparam name="T">The expected type for documents in the collection.</typeparam>
        /// <returns>The collection reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public CollectionRef<T> Collection<T>(string name) where T : IDictionary<string, object>, new()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new CollectionRef<T>(this.documentClient, name);
        }
    }
}
