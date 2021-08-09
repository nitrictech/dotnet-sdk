// [START import]
using Nitric.Api.Storage;
// [END import]

namespace Examples
{
    class DeleteExamples
    {
        public static void DeleteFile()
        {
            // [START snippet]
            var bucket = new Storage().Bucket("my-bucket");
            bucket.File("/path/to/file").Delete();
            // [END snipppet]
        }
    }
}