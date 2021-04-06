using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Event;
using Nitric.Proto.Event.v1;
using Moq;

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
        public void TestList()
        {
            var request = new TopicListRequest { };

            Mock<Proto.Event.v1.Topic.TopicClient> ec = new Mock<Proto.Event.v1.Topic.TopicClient>()
            { CallBase = true };
            ec.Setup(e => e.List(request, It.IsAny<Grpc.Core.CallOptions>()))
                .Returns(new TopicListResponse());

            ec.Verify(t => t.List(request, It.IsAny<Grpc.Core.CallOptions>()), Times.Once);
        }
    }
}
