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
﻿using System;
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
