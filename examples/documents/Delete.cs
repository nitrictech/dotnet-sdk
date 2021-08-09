// [START import]
using System.Collections.Generic;
using Nitric.Api.Document;
// [END import]
namespace Examples
{
    class DeleteExample
    {
        public static void DeleteFile()
        {
            // [START snippet]
            var docs = new Documents();

            var document = docs.Collection<Dictionary<string, object>>("products").Doc("nitric");

            document.Delete();
            // [END snippet]
        }
    }
}