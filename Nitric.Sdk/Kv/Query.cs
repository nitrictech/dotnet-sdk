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
using ProtoClient = Nitric.Proto.KeyValue.v1.KeyValue.KeyValueClient;
using Nitric.Proto.KeyValue.v1;
using Nitric.Api.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nitric.Api.KeyValue
{
    public class Query<T>
    {
        public KeyValueClient<T>.Builder<T> Builder { get; }
        public List<Expression> Expressions = new List<Expression>();
        public Dictionary<string, object> PagingToken { get; private set; }
        public int Limit { get; private set; }

        public Query(KeyValueClient<T>.Builder<T> builder){
            this.Builder = builder;
        }

        public Query<T> Where(string operand, string op, string value)
        {
            if (string.IsNullOrEmpty(operand))
            {
                throw new ArgumentNullException(operand);
            }
            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentNullException(op);
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value);
            }
            Expressions.Add(new Expression(operand, op, value));

            return this;
        }
        public QueryResult<T> fetch()
        {
            return new QueryResult<T>(this, false);
        }
        public QueryResult<T> fetchAll()
        {
            if (this.Limit <= 0)
            {
                this.Limit = 1000;
            }
            return new QueryResult<T>(this, false);
        }
        public override string ToString()
        {
            return GetType().Name
                    + "[expressions=" + this.Expressions
                    + ", limit=" + this.Limit
                    + ", pagingToken=" + this.PagingToken
                    + "]";
        }
    }
}
