using System;
using System.Collections;
using System.Collections.Generic;

namespace Nitric.Api.KeyValue
{
    public class PagingIterator<T> : IEnumerator<T>
    {
        private QueryResult<T> queryResult;
        private int index = 0;

        public PagingIterator(QueryResult<T> queryResult)
        {
            this.queryResult = queryResult;
        }
        public bool MoveNext()
        {
            if (index < queryResult.QueryData.Count)
            {
                index++;
                return true;
            }
            else if (index == queryResult.QueryData.Count) {
                if (queryResult.PagingToken != null)
                {
                    index = 0;

                    var request = queryResult.BuildKeyValueRequest(queryResult.Query.Expressions);
                    var response = queryResult.Query.Builder.client.Query(request);

                    queryResult.LoadPageData(response);

                    return queryResult.QueryData.Count > 0;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public void Reset()
        {
            index = 0;
        }

        T IEnumerator<T>.Current
        {
            get { return queryResult.QueryData[index]; }
        }

        object IEnumerator.Current
        {
            get { return queryResult.QueryData[index]; }
        }

        public T Current()
        {
            return queryResult.QueryData[index];
        }

        void IDisposable.Dispose()
        {
            //No resources to dispose
        }
    }
}
