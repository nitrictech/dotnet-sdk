// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class QueryFilterExample
    {
        public static void QueryFilter()
        {
            // [START snippet]
            var docs = new Documents();

            var query = docs.Collection<Dictionary<string, object>>("Customers").Query()
                .Where("country", "==", "US")
                .Where("age", ">=", "21");

            var results = query.Fetch();
            // [END snippet]
        }
    }
}
