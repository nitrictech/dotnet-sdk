using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Nitric.Faas;

namespace Nitric.Test.Faas
{
    [TestClass]
    public class ResponseTest
    {
        [TestMethod]
        public void TestHttpToGrpc()
        {
            var ctx = new HttpResponseContext()
                .SetStatus(200)
                .AddHeader("x-nitric-testing", "test");
            
            var response = new Response(
              Encoding.UTF8.GetBytes("Hello World"),
              ctx
            );

            var triggerResponse = response.ToGrpcTriggerResponse();

            Assert.AreEqual(triggerResponse.Data.ToStringUtf8(), "Hello World");
            Assert.IsNotNull(triggerResponse.Http);
            Assert.AreEqual(triggerResponse.Http.Status, 200);
            Assert.AreEqual(triggerResponse.Http.Headers["x-nitric-testing"], "test");
        }

        public void TestTopicToGrpc()
        {
            var ctx = new TopicResponseContext()
                .SetSuccess(true);
            
            var response = new Response(
              Encoding.UTF8.GetBytes("Hello World"),
              ctx
            );

            var triggerResponse = response.ToGrpcTriggerResponse();

            Assert.AreEqual(triggerResponse.Data.ToStringUtf8(), "Hello World");
            Assert.IsNotNull(triggerResponse.Topic);
            Assert.AreEqual(triggerResponse.Topic.Success, true);
        }
    }
}