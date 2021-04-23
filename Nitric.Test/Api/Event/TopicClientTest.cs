using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Event;
using Nitric.Proto.Event.v1;
using Moq;
using System.Collections.Generic;
using Grpc.Core;

namespace Nitric.Test.Api.Event
{
    [TestClass]
    public class TopicClientTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var topic = new TopicClient.Builder()
                .Build();
            Assert.IsNotNull(topic);
        }
        [TestMethod]
        public void TestListIsReturned()
        {
            List<NitricTopic> topics = new List<NitricTopic>();
            var topic = new NitricTopic();
            topic.Name = "Customers";
            topics.Add(topic);

            var listResponse = new TopicListResponse();
            listResponse.Topics.AddRange(topics);

            Mock<Proto.Event.v1.Topic.TopicClient> ec = new Mock<Proto.Event.v1.Topic.TopicClient>();
            ec.Setup(e => e.List(It.IsAny<TopicListRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(listResponse)
                .Verifiable();

            var topicClient = new TopicClient.Builder()
                .Client(ec.Object)
                .Build();

            var response = topicClient.List();

            Assert.IsTrue(response[0].Name == "Customers");

            ec.Verify(t => t.List(It.IsAny<TopicListRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()));
        }
        [TestMethod]
        public void TestListDoesNotExist()
        {
            Mock<Proto.Event.v1.Topic.TopicClient> ec = new Mock<Proto.Event.v1.Topic.TopicClient>();
            ec.Setup(e => e.List(It.IsAny<TopicListRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The list does not exist")))
                .Verifiable();

            var topicClient = new TopicClient.Builder()
                .Client(ec.Object)
                .Build();

            try
            {
                var response = topicClient.List();
                Assert.IsTrue(false);
            } catch (RpcException re) {
                Assert.AreEqual("Status(StatusCode=\"NotFound\", Detail=\"The list does not exist\")", re.Message);
            }

            ec.Verify(t => t.List(It.IsAny<TopicListRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()));
        }
    }
}
