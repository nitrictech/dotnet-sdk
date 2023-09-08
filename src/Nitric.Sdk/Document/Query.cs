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
using Util = Nitric.Sdk.Common.Util;

namespace Nitric.Sdk.Document
{
    /// <summary>
    /// A query object used to construct document queries.
    /// </summary>
    /// <typeparam name="T">The expected type of the found documents.</typeparam>
    public class Query<T>
    {
        internal readonly DocumentServiceClient DocumentClient;
        internal readonly AbstractCollection<T> Collection;
        internal readonly List<Expression> Expressions;
        internal object PagingToken;
        internal int ResultsLimit;

        internal Query(DocumentServiceClient documentClient, AbstractCollection<T> collection)
        {
            this.DocumentClient = documentClient;
            this.Collection = collection;
            this.Expressions = new List<Expression>();
            this.PagingToken = new Dictionary<string, string>();
            this.ResultsLimit = 0;
        }

        /// <summary>
        /// Add a 'where' clause to this query.
        /// </summary>
        /// <param name="operand">The key of the document to compare with.</param>
        /// <param name="op">The comparison type.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The query with the 'where' claus added.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Query<T> Where(string operand, string op, string value)
        {
            if (string.IsNullOrEmpty(operand))
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentNullException(nameof(op));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Expressions.Add(new Expression(operand, op, value));
            return this;
        }

        /// <summary>
        /// Add a 'where' clause to this query.
        /// </summary>
        /// <param name="operand">The key of the document to compare with.</param>
        /// <param name="op">The comparison type.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The query with the 'where' claus added.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Query<T> Where(string operand, string op, double value)
        {
            if (string.IsNullOrEmpty(operand))
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentNullException(nameof(op));
            }

            this.Expressions.Add(new Expression(operand, op, value));
            return this;
        }

        /// <summary>
        /// Add a 'where' clause to this query.
        /// </summary>
        /// <param name="operand">The key of the document to compare with.</param>
        /// <param name="op">The comparison type.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The query with the 'where' claus added.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Query<T> Where(string operand, string op, int value)
        {
            if (string.IsNullOrEmpty(operand))
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentNullException(nameof(op));
            }

            this.Expressions.Add(new Expression(operand, op, value));
            return this;
        }

        /// <summary>
        /// Add a 'where' clause to this query.
        /// </summary>
        /// <param name="operand">The key of the document to compare with.</param>
        /// <param name="op">The comparison type.</param>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The query with the 'where' claus added.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Query<T> Where(string operand, string op, bool value)
        {
            if (string.IsNullOrEmpty(operand))
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentNullException(nameof(op));
            }

            this.Expressions.Add(new Expression(operand, op, value));
            return this;
        }

        /// <summary>
        /// Query results starting from an existing paging token.
        ///
        /// Used when paging the results of a query for requesting subsequent pages.
        /// </summary>
        /// <param name="pagingToken">A paging token returned from a previous query.</param>
        /// <returns>The query with the paging token added.</returns>
        public Query<T> PagingFrom(object pagingToken)
        {
            this.PagingToken = pagingToken;
            return this;
        }

        /// <summary>
        /// Limit the number of results returned by this query.
        /// </summary>
        /// <param name="limit">A positive integer represent the maximum number of results to return.</param>
        /// <returns></returns>
        public Query<T> Limit(int limit)
        {
            this.ResultsLimit = Math.Max(0, limit);
            return this;
        }

        /// <summary>
        /// Retrieve a page of query results.
        /// </summary>
        /// <returns>Results</returns>
        public QueryResult<T> Fetch()
        {
            return new QueryResult<T>(this, false);
        }

        /// <summary>
        /// Returns a string with details about this query.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetType().Name
                   + "[collection=" + Collection
                   + ", expressions=" + Expressions
                   + ", limit=" + ResultsLimit
                   + ", pagingToken=" + PagingToken
                   + "]";
        }
    }

    /// <summary>
    /// Represents the results of an executed query.
    /// </summary>
    /// <typeparam name="T">The expected type of the found documents.</typeparam>
    public class QueryResult<T>
    {
        private readonly Query<T> query;
        private readonly bool paginateAll;

        /// <summary>
        /// An optional token from a previous query, indicates where to start when retrieving additional results.
        /// </summary>
        public object PagingToken { get; private set; }

        /// <summary>
        /// The results returned from the query.
        /// </summary>
        public List<Document<T>> Documents { get; private set; }

        internal QueryResult(Query<T> query, bool paginateAll)
        {
            this.query = query;
            this.paginateAll = paginateAll;
            this.PagingToken = query.PagingToken;

            var request = BuildDocQueryRequest(this.query.Expressions);

            try
            {
                var response = this.query.DocumentClient.Query(request);
                LoadPageData(response);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.FromRpcException(re);
            }
        }

        private DocumentQueryRequest BuildDocQueryRequest(List<Expression> expressions)
        {
            var request = new DocumentQueryRequest
            {
                Collection = this.query.Collection.ToGrpcCollection()
            };

            foreach (var expression in expressions)
            {
                request.Expressions.Add(expression.ToGrpcExpression());
            }

            request.Limit = this.query.ResultsLimit;
            if (this.PagingToken == null) return request;

            if (!(this.PagingToken is IDictionary<string, string> token))
            {
                throw new ArgumentException("Invalid paging token provided!");
            }

            foreach (var kv in token)
            {
                request.PagingToken.Add(kv.Key, kv.Value);
            }

            return request;
        }

        private void LoadPageData(DocumentQueryResponse response)
        {
            Documents = new List<Document<T>>(response.Documents.Count);
            var collection = this.query.Collection.ToGrpcCollection();
            foreach (var doc in response.Documents)
            {
                var dict = Util.Utils.ObjToDict(doc.Content);

                if (typeof(T).IsAssignableFrom(dict.GetType()))
                {
                    Documents.Add(new Document<T>(
                        new DocumentRef<T>(
                            this.query.DocumentClient,
                            this.query.Collection,
                            this.query.Collection.ParentKey.Id
                        ),
                        (T)dict)
                    );
                }
                else
                {
                    Documents.Add(new Document<T>(
                        new DocumentRef<T>(
                            this.query.DocumentClient,
                            this.query.Collection,
                            this.query.Collection.ParentKey.Id
                        ),
                        (T)Util.Utils.ObjToDict(dict)
                    ));
                }
            }

            this.PagingToken = response.PagingToken.Clone();
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

            switch (value)
            {
                case string s:
                    expressionValue.StringValue = s;
                    break;
                case double d:
                    expressionValue.DoubleValue = d;
                    break;
                case int i:
                    expressionValue.IntValue = i;
                    break;
                case bool b:
                    expressionValue.BoolValue = b;
                    break;
                default:
                {
                    var msg = value.GetType().Name
                              + "type is not supported. Please use: string, double, integer, boolean";
                    throw new ArgumentException(msg);
                }
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
