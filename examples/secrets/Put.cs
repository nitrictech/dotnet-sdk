// [START import]
using Nitric.Api.Secret;
// [END import]
namespace Examples
{
    class PutExample
    {
        public static void PutSecret()
        {
            // [START snippet]
            var secrets = new Secrets();

            var newPassword = "qxGJp9rWMbYvPEsNFXzukQa!";

            // Store the new password value, making it the latest version
            secrets.Secret("database.password").Put(newPassword);
            // [END snippet]
        }
    }
}