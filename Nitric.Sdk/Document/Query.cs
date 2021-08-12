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
using GrpcExpression = Nitric.Proto.Document.v1.Expression;
using ExpressionValue = Nitric.Proto.Document.v1.ExpressionValue;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using Nitric.Proto.Document.v1;
using Collection = Nitric.Proto.Document.v1.Collection;
using Util = Nitric.Api.Common.Util;

namespace Nitric.Api.Document
{
    public class Query<T> where T : IDictionary<string, object>, new()
    {
        internal DocumentServiceClient documentClient;
        internal CollectionRef<T> collection;
        internal List<Expression> expressions;
        internal object pagingToken;
        internal int limit;

        internal Query(DocumentServiceClient documentClient, CollectionRef<T> collection)
        {
            this.documentClient = documentClient;
            this.collection = collection;
            this.expressions = new List<Expression>();
            this.pagingToken = new Dictionary<string, string>();
            this.limit = 0;
        }
        public Query<T> Where(string operand, string op, string value)
        {
            if (string.IsNullOrEmpty(operand))
            {
                throw new ArgumentNullException("operand");
            }
            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentNullException("op");
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            this.expressions.Add(new Expression(operand, op, value));
            return this;
        }

        public Query<T> PagingFrom(object pagingToken)
        {
            this.pagingToken = pagingToken;
            return this;
        }
        public Query<T> Limit(int limit)
        {
            this.limit = Math.Max(0, limit);
            return this;
        }
        public QueryResult<T> Fetch()
        {
            return new QueryResult<T>(this, false);
        }
        public QueryResult<T> FetchAll()
        {
            if (this.limit <= 0)
            {
                this.limit = 1000;
            }
            return new QueryResult<T>(this, true);
        }
        public override string ToString()
        {
            return this.GetType().Name
                    + "[collection=" + collection
                    + ", expressions=" + expressions
                    + ", limit=" + limit
                    + ", pagingToken=" + pagingToken
                    + "]";
        }
    }
    public class QueryResult<T> where T : IDictionary<string, object>, new()
    {
        internal readonly Query<T> query;
        internal readonly bool paginateAll;
        public object PagingToken { get; private set; }
        public List<Document<T>> QueryData { get; private set; }

        internal QueryResult(Query<T> query, bool paginateAll)
        {
            this.query = query;
            this.paginateAll = paginateAll;
            this.PagingToken = query.pagingToken;

            var request = BuildDocQueryRequest(this.query.expressions);

            try
            {
                DocumentQueryResponse response = this.query.documentClient.Query(request);
                LoadPageData(response);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.Exceptions[re.StatusCode](re.Message);
            }
        }

        internal DocumentQueryRequest BuildDocQueryRequest(List<Expression> expressions)
        {
            var request = new DocumentQueryRequest();
            request.Collection = this.query.collection.ToGrpcCollection();

            foreach (Expression expression in expressions)
            {
                request.Expressions.Add(expression.ToGrpcExpression());
            }
            request.Limit = this.query.limit;
            if (this.PagingToken != null)
            {
                if (!typeof(IDictionary<string, string>).IsAssignableFrom(this.PagingToken.GetType()))
                {
                    throw new ArgumentException("Invalid paging token provided!");
                }
                foreach (KeyValuePair<string, string> kv in (IDictionary<string, string>)this.PagingToken)
                {
                    request.PagingToken.Add(kv.Key, kv.Value);
                }
            }
            return request;
        }

        internal void LoadPageData(DocumentQueryResponse response)
        {
            QueryData = new List<Document<T>>(response.Documents.Count);
            var collection = this.query.collection.ToGrpcCollection();
            foreach (var doc in response.Documents)
            {
                var dict = Util.ObjToDict(doc.Content);

                if (typeof(T).IsAssignableFrom(dict.GetType()))
                {
                    QueryData.Add(new Document<T>(
                        new DocumentRef<T>(
                            this.query.documentClient,
                            this.query.collection,
                            this.query.collection.ParentKey.id
                        ),
                        (T)Util.DictToCollection<T>(dict))
                    );
                }
                else
                {
                    QueryData.Add(new Document<T>(
                        new DocumentRef<T>(
                            this.query.documentClient,
                            this.query.collection,
                            this.query.collection.ParentKey.id
                        ),
                        (T)Util.DictToCollection<T>(Util.ObjToDict(dict)))
                    );
                }
            }
            this.PagingToken = Util.CollectionToDict(response.PagingToken);
        }
    }
    class Expression
    {
        readonly string operand;
        readonly string op;
        readonly object value;

        internal Expression(string operand, string op, object value)
        {
            this.operand = operand;
            this.op = op;
            this.value = value;
        }

        internal GrpcExpression ToGrpcExpression()
        {
            var expression = new GrpcExpression();
            var expressionValue = new ExpressionValue();

            expression.Operand = operand;
            expression.Operator = op;

            if (value.GetType() == typeof(string))
            {
                expressionValue.StringValue = (string)value;
            }
            else if (value.GetType() == typeof(double))
            {
                expressionValue.DoubleValue = (double)value;
            }
            else if (value.GetType() == typeof(int))
            {
                expressionValue.IntValue = (int)value;
            }
            else if (value.GetType() == typeof(bool))
            {
                expressionValue.BoolValue = (bool)value;
            }
            else
            {
                string msg = value.GetType().Name
                    + "type is not supported. Please use: string, double, integer, boolean";
                throw new ArgumentException(msg);
            }
            expression.Value = expressionValue;
            return expression;
        }

        public override string ToString()
        {
            return this.GetType().Name
                    + "[operand=" + this.operand
                    + ", operator=" + this.op
                    + ", value=" + this.value
                    + "]";
        }
    }
}
