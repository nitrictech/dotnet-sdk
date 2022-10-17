// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class QueryLimitsExample
    {
        public static void QueryLimits()
        {
            // [START snippet]
            var docs = new DocumentsClient();

            var query = docs.Collection<Dictionary<string, object>>("Customers")
                    .Query()
                    .Limit(1000);

            var results = query.Fetch();
            // [END snippet]
        }
    }
}
