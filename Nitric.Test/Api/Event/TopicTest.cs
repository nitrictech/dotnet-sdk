using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Nitric.Test.Api.Event
{
    [TestClass]

    public class TopicTest
    {
        [TestMethod]
        public void TestBuild()
        {

            var topic = new Nitric.Api.Event.Topic.Builder()
                .Name("topic")
                .Build();

            Assert.IsNotNull(topic);
            Assert.AreEqual("topic", topic.Name);

            try
            {
                topic = new Nitric.Api.Event.Topic.Builder()
                .Build();
                Assert.IsTrue(false);
            } catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'name')", ane.Message);
            }
        }
        [TestMethod]
        public void TestToString()
        {
            var topic = new Nitric.Api.Event.Topic.Builder()
                .Name("Test Topic")
                .Build();
            Assert.AreEqual("Topic[name=Test Topic]", topic.ToString());
        }
    }
}
