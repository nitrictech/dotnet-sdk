// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class SubDocQueryExample
    {
        public static void QueryDocCol()
        {
            // [START snippet]
            var docs = new Documents();

            var query = docs.Collection<Dictionary<string, object>>("customers")
                    .Doc("apple")
                    .Collection("Orders")
                    .Query();

            var results = query.Fetch();
            // [END snippet]
        }
    }
}
