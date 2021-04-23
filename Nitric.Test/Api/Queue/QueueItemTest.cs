using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Nitric.Test.Api.Queue
{
    [TestClass]
    public class QueueItemTest
    {
        [TestMethod]
        public void TestBuild()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("name", "value");
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            var queueItem = new Nitric.Api.Queue.Task
                .Builder()
                .LeaseID("1")
                .RequestID("2")
                .Payload(payloadStruct)
                .PayloadType("payload type")
                .Build();

            Assert.IsNotNull(queueItem);
            Assert.AreEqual("1", queueItem.LeaseID);
            Assert.AreEqual("2", queueItem.Event.RequestId);
            Assert.AreEqual("payload type", queueItem.Event.PayloadType);
            Assert.AreEqual(payloadStruct, queueItem.Event.Payload);
        }
        [TestMethod]
        public void TestToString()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("name", "value");
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            var task = new Nitric.Api.Queue.Task
                .Builder()
                .LeaseID("1")
                .RequestID("2")
                .Payload(payloadStruct)
                .PayloadType("payload type")
                .Build();

            Assert.AreEqual("Task[event=Event[id=2, " +
                "payloadType=payload type, " +
                "payload={ \"name\": \"value\" }], " +
                "leaseId=1]", task.ToString());
        }
    }
}
