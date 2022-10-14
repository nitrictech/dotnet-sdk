// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class QueryExample
    {
        public static void Query()
        {
            // [START snippet]
            var docs = new Documents();

            var query = docs.Collection<Dictionary<string, object>>("customers").Query();

            var results = query.Fetch();
            // [END snippet]
        }
    }
}
