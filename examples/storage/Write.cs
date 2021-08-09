// [START import]
using System.Text;
using Nitric.Api.Storage;
// [END import]

namespace Examples
{
    class WriteExample
    {
        public static void WriteFile()
        {
            // [START snippet]
            var data = Encoding.UTF8.GetBytes("Hello World");

            var bucket = new Storage().Bucket("my-bucket");
            bucket.File("/path/to/file").Write(data);
            // [END snipppet]
        }
    }
}