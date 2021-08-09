// [START import]
using System.Collections.Generic;
using Nitric.Api.Document;
// [END import]
namespace Examples
{
    class RefsExample
    {
        public static void RefDocs()
        {
            // [START snippet]
            var docs = new Documents();

            // create a reference to a collection named 'products'
            var collection = docs.Collection<Dictionary<string, object>>("products");

            // create a reference to a document with the id 'nitric'
            var nitric = collection.Doc("nitric");
            // [END snippet]
        }
    }
}