/* TEST CURRENTLY WON'T WORK WITHOUT SUBCOLLECTION QUERYS
// [START import]
using System.Collections.Generic;
using Nitric.Api.Document;
// [END import]
namespace DocumentsExamples
{
    class SubColQueryExample
    {
        public static void QuerySubCol()
        {
            // [START snippet]
            var docs = new Documents();

            var query = docs.Collection<Dictionary<string, object>>("customers")
                    .Collection("Orders")
                    .Query();

            var results = query.Fetch(); ;
            // [END snippet]
        }
    }
}
*/