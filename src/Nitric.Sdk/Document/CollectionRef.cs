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
using Util = Nitric.Sdk.Common.Util;
using System.Collections.Generic;
using Constants = Nitric.Sdk.Common.Constants;

namespace Nitric.Sdk.Document
{
    public class CollectionRef<T> : AbstractCollection<T> where T : IDictionary<string, object>, new()
    {
        internal CollectionRef(DocumentServiceClient documentClient, string name, Key<T> parentKey = null)
        : base(documentClient, name, parentKey) { }

        public DocumentRef<T> Doc(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
            {
                throw new ArgumentNullException("documentId");
            }
            return new DocumentRef<T>(
                this.documentClient,
                this,
                documentId);
        }
        public CollectionGroup<T> Collection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            if (Util.Utils.CollectionDepth(this.ToGrpcCollection()) >= Constants.DepthLimit)
            {
                throw new NotSupportedException("Currently sub-collections are only able to be nested " + Constants.DepthLimit + "deep");
            }
            var parentKey = new Key<T>(this, "");
            return new CollectionGroup<T>(
                this.documentClient,
                name,
                parentKey);
        }
    }
}

