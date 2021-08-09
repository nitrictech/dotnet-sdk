// [START import]
using Nitric.Api.Secret;
// [END import]
namespace Examples
{
    class AccessExample
    {
        public static void AccessSecret()
        {
            // [START snippet]
            var secrets = new Secrets();

            var value = secrets.Secret("database.password")
                .Latest()
                .Access();

            var password = value.ValueText;
            // [END snippet]
        }
    }
}