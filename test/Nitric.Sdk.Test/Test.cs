using Nitric.Sdk.Resource;
using Nitric.Sdk.Service;

namespace Test
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var app = Nitric.Sdk.Nitric.Api("Blah");

            var oidc = Nitric.Sdk.Nitric.OidcRule("ruleName", "issuer", new string[] { "audiences" });

            var route = app.Route("/hello", new RouteOptions { Security = new OidcOptions[] { oidc("user.read") } });

            route.Get((HttpContext ctx) =>
            {
                ctx.Res.Text("Hello World");
                return ctx;
            });

        }
    }
}
