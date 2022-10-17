// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class GetExample
    {
        public static void GetFile()
        {
            // [START snippet]
            var docs = new DocumentsClient();

            var document = docs.Collection<Dictionary<string, object>>("products").Doc("nitric");

            var product = document.Get();
            // [END snippet]
        }
    }
}
