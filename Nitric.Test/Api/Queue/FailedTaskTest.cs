using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.Protobuf.WellKnownTypes;
namespace Nitric.Test.Api.Queue
{
    [TestClass]
    public class FailedTaskTest
    {
        [TestMethod]
        public void TestBuild()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("name", "value");
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            var failedTask = new Nitric.Api.Queue.FailedTask
                .Builder()
                .RequestId("1")
                .PayloadType("payload type")
                .Payload(payloadStruct)
                .Message("message")
                .Build();

            Assert.IsNotNull(failedTask);
            Assert.AreEqual("1", failedTask.Event.RequestId);
            Assert.AreEqual("payload type", failedTask.Event.PayloadType);
            Assert.AreEqual(payloadStruct, failedTask.Event.Payload);
            Assert.AreEqual("message", failedTask.Message);
        }
        [TestMethod]
        public void TestToString()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("name", "value");
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            var failedTask = new Nitric.Api.Queue.FailedTask
                .Builder()
                .RequestId("1")
                .PayloadType("payload type")
                .Payload(payloadStruct)
                .Message("message")
                .Build();

            Assert.AreEqual("FailedTask[event=Event[id=1, " +
                "payloadType=payload type, " +
                "payload={ \"name\": \"value\" }], " +
                "message=message]", failedTask.ToString());
        }
    }
}
