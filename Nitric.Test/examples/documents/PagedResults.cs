// [START import]
using System.Collections.Generic;
using Nitric.Api.Document;
// [END import]
namespace Examples
{
    class PagedResultsExample
    {
        public static void PagedResults()
        {
            // [START snippet]
            var docs = new Documents();

            var query = docs.Collection<Dictionary<string, object>>("Customers")
                .Query()
                .Where("active", "==", "true")
                .Limit(100);

            // Fetch first page
            var results = query.Fetch();

            // Fetch next page
            results = query.PagingFrom(results.PagingToken).Fetch();
            // [END snippet]
        }
    }
}