// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Document;
// [END import]
namespace Examples
{
    class SetExample
    {
        public static void SetFile()
        {
            // [START snippet]
            var docs = new DocumentsClient();

            var document = docs.Collection<Dictionary<string, object>>("products").Doc("nitric");

            var product = new Dictionary<string, object>();
            product.Add("id", "nitric");
            product.Add("name", "Nitric Framework");
            product.Add("description", "A development framework");

            document.Set(product);
            // [END snippet]
        }
    }
}
