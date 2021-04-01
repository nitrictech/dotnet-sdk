using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nitric.Test.Api.Common
{
    [TestClass]
    public class EventTest
    {
        [TestMethod]
        public void TestBuild()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("name", "value");

            var eventTest = new Nitric.Api.Common.Event.Builder()
                .RequestId("id")
                .PayloadType("payloadType")
                .PayloadStruct(Nitric.Api.Common.Util.ObjectToStruct(payload))
                .Build();
                
            Assert.IsNotNull(eventTest);
            Assert.AreEqual("id", eventTest.RequestId);
            Assert.AreEqual("payloadType", eventTest.PayloadType);
            Assert.AreEqual(Nitric.Api.Common.Util.ObjectToStruct(payload), eventTest.PayloadStruct);
            Assert.AreEqual("Event[id=id, payloadType=payloadType, payload={ \"name\": \"value\" }]", eventTest.ToString());
        }
    }
}
