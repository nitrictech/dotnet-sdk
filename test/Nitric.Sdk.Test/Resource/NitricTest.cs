// using System.Collections.Generic;
// using static Nitric.Sdk.Nitric;
//
// var api = Api("public");
//
// api.Delete("/customer/:customer_id", ctx =>
// {
//     // perform the delete and send a response.
//     ctx.res.Json(
//     new Dictionary<string, object> {
//         { "text", "test"},
//         { "number", 123},
//     });
//     return ctx;
// });
//
// Run();

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Nitric.Sdk.Resource;
using Xunit;

namespace Nitric.Sdk.Test.Resource
{
    public class NitricTest
    {
        [Fact]
        public void TestRegisterApi()
        {
            var api = Nitric.Api("test");
            var topic = Nitric.Topic("stuff");
            var pub = topic.With(TopicPermission.Publishing);

            api.Get("/one", ctx =>
            {
                var payload = new Dictionary<string, object>
                {
                    { "text", "test" },
                    { "number", 123 },
                };
                // var id = pub.Publish(new Event.Event {Payload = payload});
                // ctx.res.Text("id");
                ctx.Res.Json(payload);
                return ctx;
            });

            api.Get("/two", ctx =>
            {
                Console.Out.WriteLine("it's working");
                ctx.Res.Body = Encoding.UTF8.GetBytes("other thing");
                return ctx;
            });

            // topic.Subscribe(ctx =>
            // {
            //     System.Console.WriteLine(JsonSerializer.Serialize(ctx.req.Payload));
            //     return ctx;
            // });

            Nitric.Run();
        }
    }
}
