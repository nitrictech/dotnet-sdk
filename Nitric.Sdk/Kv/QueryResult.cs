using System;
using System.Collections;
using System.Collections.Generic;
using Nitric.Proto.KeyValue.v1;
using Util = Nitric.Api.Common.Util;
namespace Nitric.Api.KeyValue
{
    public class QueryResult<T> : IEnumerable<T>
    {
        public Query<T> Query { get; private set; }
        public bool PaginateAll { get; private set; }
        public Dictionary<string, object> PagingToken { get; private set; }
        public List<T> QueryData { get; private set; }

        public QueryResult(Query<T> query, bool paginateAll)
        {
            this.Query = query;
            this.PagingToken = query.PagingToken;
            this.PaginateAll = paginateAll;

            var request = BuildKeyValueRequest(this.Query.Expressions);
            var response = this.Query.Builder.Build().Query();
        }
        public KeyValueQueryRequest BuildKeyValueRequest(List<Expression> expressions)
        {
            var requestBuilder = new KeyValueQueryRequest
            {
                Collection = this.Query.Builder.Build().Collection,
                Limit = this.Query.Limit
            };

            foreach (Expression e in expressions)
            {
                var exp = new KeyValueQueryRequest.Types.Expression
                {
                    Operand = e.Operand,
                    Operator = e.Op,
                    Value = e.Value,
                };
                requestBuilder.Expressions.Add(exp);
            }
            if (this.PagingToken != null)
            {
                requestBuilder.PagingToken.Add(Util.ObjToDict(this.PagingToken));
            }
            return requestBuilder;
        }
        //TODO
        public void LoadPageData(KeyValueQueryResponse response)
        {
            this.QueryData = new List<T>(response.Values.Count);

            foreach(var v in response.Values)
            {
                var map = Util.ObjToDict(v);

                if (map.GetType().IsAssignableFrom(Query.Builder.Build().Type)){
                    QueryData.Add((T)(object)map);
                }
                else
                {
                    var value = ""; //Convert from dict to an object of type Query.Builder.type
                    QueryData.Add((T)(object)value);
                }
            }

            Dictionary<string, object> resultPagingToken = (response.PagingToken != null)
                ? Util.ObjToDict(response.PagingToken) : null;

            this.PagingToken = (resultPagingToken != null && resultPagingToken.Values != null)
                ? resultPagingToken : null;
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (!this.PaginateAll)
            {
                return QueryData.GetEnumerator();
            } else {
                return new PagingIterator<T>(this);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
