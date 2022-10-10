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
using Collection = Nitric.Proto.Document.v1.Collection;
namespace Nitric.Sdk.Document
{
    public class AbstractCollection<T> where T : IDictionary<string, object>, new()
    {
        public readonly string Name;
        public readonly Key<T> ParentKey;
        internal DocumentServiceClient documentClient;

        public AbstractCollection(DocumentServiceClient documentClient, string name, Key<T> parentKey = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this.documentClient = documentClient;
            this.Name = name;
            this.ParentKey = parentKey;
        }
        public Query<T> Query()
        {
            return new Query<T>(this.documentClient, this);
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
    }
}
