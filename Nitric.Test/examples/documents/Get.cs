// [START import]
using System.Collections.Generic;
using Nitric.Api.Document;
// [END import]
namespace Examples
{
    class GetExample
    {
        public static void GetFile()
        {
            // [START snippet]
            var docs = new Documents();

            var document = docs.Collection<Dictionary<string, object>>("products").Doc("nitric");

            var product = document.Get();
            // [END snippet]
        }
    }
}