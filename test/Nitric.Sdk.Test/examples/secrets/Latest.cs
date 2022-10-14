// [START import]
using Nitric.Sdk.Secret;
// [END import]
namespace Examples
{
    class LatestExample
    {
        public static void LatestSecret()
        {
            // [START snippet]
            var secrets = new Secrets();

            var latest = secrets.Secret("database.password").Latest();
            // [END snippet]
        }
    }
}
