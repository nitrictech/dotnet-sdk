// [START import]
using Nitric.Api.Storage;
// [END import]

namespace Examples
{
    class ReadExample
    {
        public static void ReadFile()
        {
            // [START snippet]
            var bucket = new Storage().Bucket("my-bucket");
            var file = bucket.File("/path/to/file").Read();
            // [END snipppet]
        }
    }
}