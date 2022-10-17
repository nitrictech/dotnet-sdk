// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class DeleteExample
    {
        public static void DeleteFile()
        {
            // [START snippet]
            var docs = new DocumentsClient();

            var document = docs.Collection<Dictionary<string, object>>("products").Doc("nitric");

            document.Delete();
            // [END snippet]
        }
    }
}
