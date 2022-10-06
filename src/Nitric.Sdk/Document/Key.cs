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
using Collection = Nitric.Proto.Document.v1.Collection;
using GrpcKey = Nitric.Proto.Document.v1.Key;

namespace Nitric.Api.Document
{
    public class Key<T> where T : IDictionary<string, object>, new()
    {
        public readonly AbstractCollection<T> collection;
        public readonly string id;

        public Key(AbstractCollection<T> collection, string id = null)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.collection = collection;
            this.id = id != null ? id : "";
        }
        public GrpcKey ToKey()
        {
            return new GrpcKey
            {
                Collection = this.collection.ToGrpcCollection(),
                Id = this.id,
            };
        }
        public override string ToString()
        {
            return this.GetType().Name + "[collection=" + collection + ", id=" + id + "]";
        }
    }
}
