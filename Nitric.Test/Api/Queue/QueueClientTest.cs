using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Proto.Queue.v1;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using Moq;
using Google.Protobuf.WellKnownTypes;
namespace Nitric.Test.Api.Queue
{
    [TestClass]
    public class QueueClientTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var queueClient = new Nitric.Api.Queue.QueueClient
                .Builder()
                .Build();

            Assert.IsNotNull(queueClient);
        }
        [TestMethod]
        public void TestSendBatch()
        {
            var request = new QueueSendBatchRequest { Queue = "queue" };
            request.Tasks.AddRange(new RepeatedField<NitricTask>());

            Mock<Proto.Queue.v1.Queue.QueueClient> ec = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            ec.Setup(e => e.SendBatch(request, default))
                .Returns(new QueueSendBatchResponse())
                .Verifiable();

            ec.Verify(t => t.SendBatch(request, default), Times.Once);
        }
        [TestMethod]
        public void TestRecieve()
        {
            var request = new QueueReceiveRequest { Queue = "queue", Depth = 2 };

            Mock<Proto.Queue.v1.Queue.QueueClient> ec = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            ec.Setup(e => e.Receive(request, default))
                .Returns(new QueueReceiveResponse())
                .Verifiable();

            ec.Verify(t => t.Receive(request, default), Times.Once);
        }
        [TestMethod]
        public void TestWireToQueueItem()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            NitricTask nitricTask = new NitricTask();
            nitricTask.LeaseId = "1";
            nitricTask.Id = "2";
            nitricTask.Payload = payloadStruct;
            nitricTask.PayloadType = "payload type";

            var queueItem = new Nitric.Api.Queue.QueueItem
                .Builder()
                .RequestID(nitricTask.Id)
                .PayloadType(nitricTask.PayloadType)
                .Payload(nitricTask.Payload)
                .LeaseID(nitricTask.LeaseId)
                .Build();

            Assert.IsNotNull(queueItem);
            Assert.AreEqual("2", queueItem.Event.RequestId);
            Assert.AreEqual(payloadStruct, queueItem.Event.Payload);
            Assert.AreEqual("payload type", queueItem.Event.PayloadType);
            Assert.AreEqual("1", queueItem.LeaseID);
        }

        [TestMethod]
        public void TestEventToWire()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            var sdkEvent = new Nitric.Api.Common.Event
                .Builder()
                .RequestId("1")
                .Payload(payloadStruct)
                .PayloadType("payload type")
                .Build();

            var nitricTask = new NitricTask
            {
                Id = sdkEvent.RequestId,
                PayloadType = sdkEvent.PayloadType,
                Payload = sdkEvent.Payload
            };

            Assert.IsNotNull(nitricTask);
            Assert.AreEqual("1", nitricTask.Id);
            Assert.AreEqual("payload type", nitricTask.PayloadType);
            Assert.AreEqual(payloadStruct, nitricTask.Payload);
        }

        [TestMethod]
        public void TestWireToFailedEvent()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            var protoFailedTask = new FailedTask
            {
                Message = "message",
                Task = new NitricTask
                {
                    Id = "1",
                    PayloadType = "payload type",
                    Payload = payloadStruct
                }
            };

            var failedTask = new Nitric.Api.Queue.FailedTask.Builder()
                .RequestId(protoFailedTask.Task.Id)
                .PayloadType(protoFailedTask.Task.PayloadType)
                .Payload(protoFailedTask.Task.Payload)
                .Message(protoFailedTask.Message)
                .Build();

            Assert.IsNotNull(failedTask);
            Assert.AreEqual("message", failedTask.Message);
            Assert.AreEqual("1", failedTask.Event.RequestId);
            Assert.AreEqual("payload type", failedTask.Event.PayloadType);
            Assert.AreEqual(payloadStruct, failedTask.Event.Payload);
        }

        [TestMethod]
        public void TestWireToEvent()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            Struct payloadStruct = Nitric.Api.Common.Util.ObjectToStruct(payload);
            NitricTask nitricTask = new NitricTask();
            nitricTask.LeaseId = "1";
            nitricTask.Id = "2";
            nitricTask.Payload = payloadStruct;
            nitricTask.PayloadType = "payload type";

            var sdkEvent = new Nitric.Api.Common.Event
                .Builder()
                .RequestId(nitricTask.Id)
                .PayloadType(nitricTask.PayloadType)
                .Payload(nitricTask.Payload)
                .Build();

            Assert.IsNotNull(sdkEvent);
            Assert.AreEqual("2", sdkEvent.RequestId);
            Assert.AreEqual(payloadStruct, sdkEvent.Payload);
            Assert.AreEqual("payload type", sdkEvent.PayloadType);
        }
    }
}
