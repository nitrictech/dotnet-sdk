using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Text;
using Nitric.Proto.Faas.v1;
using Nitric.Faas;

namespace Nitric.Test.Faas
{
    [TestClass]
    public class TriggerTest
    {
        [TestMethod]
        public void TestFromGrpcTriggerRequestHttp()
        {
            var triggerRequest = new TriggerRequest();
            triggerRequest.Data = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes("Hello World"));
            triggerRequest.Http = new HttpTriggerContext();
            triggerRequest.Http.Method = "GET";
            triggerRequest.Http.Headers.Add("x-nitric-test", "test");
            triggerRequest.MimeType = "text/plain";
            triggerRequest.Http.Path = "/test/";

            var trigger = Trigger.FromGrpcTriggerRequest(triggerRequest);


            Assert.IsTrue(trigger.Context.IsHttp());
            Assert.AreEqual("Hello World", Encoding.UTF8.GetString(trigger.Data));
            Assert.AreEqual("text/plain", trigger.MimeType);
            Assert.AreEqual(trigger.Context.AsHttp().Method, "GET");
            Assert.AreEqual(trigger.Context.AsHttp().Path, "/test/");
            Assert.AreEqual(trigger.Context.AsHttp().Headers["x-nitric-test"], "test");
        }

        [TestMethod]
        public void TestFromGrpcTriggerTopic()
        {
            var triggerRequest = new TriggerRequest();
            triggerRequest.Data = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes("Hello World"));
            triggerRequest.MimeType = "text/plain";
            triggerRequest.Topic = new Nitric.Proto.Faas.v1.TopicTriggerContext();
            triggerRequest.Topic.Topic = "Test";

            var trigger = Trigger.FromGrpcTriggerRequest(triggerRequest);

            Assert.IsTrue(trigger.Context.IsTopic());
            Assert.AreEqual("Hello World", Encoding.UTF8.GetString(trigger.Data));
            Assert.AreEqual("text/plain", trigger.MimeType);
            Assert.AreEqual(trigger.Context.AsTopic().Topic, "Test");
        }
		}
}