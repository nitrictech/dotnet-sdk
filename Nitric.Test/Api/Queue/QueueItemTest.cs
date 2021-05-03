// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
