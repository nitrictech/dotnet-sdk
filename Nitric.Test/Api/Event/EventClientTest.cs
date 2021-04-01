using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Event;
using System.Collections.Generic;
using Nitric.Proto.Event.v1;
using Moq;
using Util = Nitric.Api.Common.Util;

namespace Nitric.Test.Api.Event
{
    [TestClass]
    public class EventClientTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var evt = new EventClient.Builder()
                .Build();
            Assert.IsNotNull(evt);
        }
        [TestMethod]
        public void TestPublish()
        {

            var payloadStruct = Util.ObjectToStruct(new Dictionary<string,string>());
            var evt = new NitricEvent { Id = "1", PayloadType = "payloadType", Payload = payloadStruct };
            var request = new EventPublishRequest { Topic = "topic", Event = evt };

            Mock<Proto.Event.v1.Event.EventClient> ec = new Mock<Proto.Event.v1.Event.EventClient>();
            ec.Setup(e => e.Publish(request, It.IsAny<Grpc.Core.CallOptions>()))
                .Returns(new EventPublishResponse());
        }
    };
}
