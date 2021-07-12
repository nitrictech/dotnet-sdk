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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Event;
using EventModel = Nitric.Api.Event.Event;
using System.Collections.Generic;
using Nitric.Proto.Event.v1;
using Moq;
using Util = Nitric.Api.Common.Util;

namespace Nitric.Test.Api.EventClient
{
    [TestClass]
    public class EventClientTest
    {
        [TestMethod]
        public void TestBuildEvents()
        {
            var evt = new Events();
            Assert.IsNotNull(evt);
        }
        [TestMethod]
        public void TestBuildTopicWithName()
        {
            var topic = new Events().Topic("test-topic");
            Assert.IsNotNull(topic);
            Assert.AreEqual("test-topic", topic.Name);
        }
        [TestMethod]
        public void TestBuildTopicWithoutName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Events().Topic("")
            );
            Assert.ThrowsException<ArgumentNullException>(
                () => new Events().Topic(null)
            );
        }
        [TestMethod]
        public void TestPublish()
        {
            var payloadStruct = Util.ObjToStruct(new Dictionary<string,string>());
            var evt = new NitricEvent { Id = "1", PayloadType = "payloadType", Payload = payloadStruct };
            var request = new EventPublishRequest { Topic = "test-topic", Event = evt };

            Mock<Proto.Event.v1.Event.EventClient> ec = new Mock<Proto.Event.v1.Event.EventClient>();

            ec.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new EventPublishResponse())
                .Verifiable();

            var topic = new Events(ec.Object).Topic("test-topic");

            var evtToSend = EventModel
                .NewBuilder()
                .Id("1")
                .PayloadType("payloadType")
                .Payload(new Dictionary<string, string>())
                .Build();

            var response = topic.Publish(evtToSend);

            ec.Verify(t => t.Publish(It.IsAny<EventPublishRequest>(), null,null,It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    };
}
