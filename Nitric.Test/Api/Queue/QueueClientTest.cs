using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Proto.Queue.v1;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using Moq;
using Google.Protobuf.WellKnownTypes;
using Nitric.Api.Queue;
namespace Nitric.Test.Api.Queue
{
    [TestClass]
    public class QueueClientTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var queueClient = new QueueClient
                .Builder()
                .Build();

            Assert.IsNotNull(queueClient);
        }
        [TestMethod]
        public void TestSendBatchWithFailedTasks()
        {
            //Setting up failed tasks to then return later
            NitricTask failedTaskTask = new NitricTask();
            failedTaskTask.Id = "0";
            failedTaskTask.LeaseId = "1";
            failedTaskTask.Payload = Nitric.Api.Common.Util.ObjectToStruct(new Dictionary<string, string>());
            failedTaskTask.PayloadType = "Dictionary";

            Proto.Queue.v1.FailedTask failedTask = new Proto.Queue.v1.FailedTask();
            failedTask.Message = "I am a failed task... I failed my task";
            failedTask.Task = failedTaskTask;

            List<Proto.Queue.v1.FailedTask> failedTasks = new List<Proto.Queue.v1.FailedTask>();
            failedTasks.Add(failedTask);

            var queueBatchResponse = new QueueSendBatchResponse();
            queueBatchResponse.FailedTasks.AddRange(failedTasks);

            Mock<Proto.Queue.v1.Queue.QueueClient> ec = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            ec.Setup(e => e.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueBatchResponse)
                .Verifiable();

            var queueClient = new QueueClient.Builder()
                .Client(ec.Object)
                .Build();

            var response = queueClient.SendBatch("queue", new List<Nitric.Api.Common.Event>());

            Assert.AreEqual("I am a failed task... I failed my task", response.getFailedTasks()[0].Message);

            ec.Verify(t => t.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSendBatchWithNoFailedTasks()
        {
            Mock<Proto.Queue.v1.Queue.QueueClient> ec = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            ec.Setup(e => e.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueSendBatchResponse())
                .Verifiable();

            var queueClient = new QueueClient.Builder()
                .Client(ec.Object)
                .Build();

            var response = queueClient.SendBatch("queue", new List<Nitric.Api.Common.Event>());

            Assert.AreEqual(0, response.getFailedTasks().Count);

            ec.Verify(t => t.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReceiveTasks()
        {
            NitricTask taskToReturn = new NitricTask();
            taskToReturn.Id = "32";
            taskToReturn.LeaseId = "1";
            taskToReturn.Payload = Nitric.Api.Common.Util.ObjectToStruct(new Dictionary<string, string>());
            taskToReturn.PayloadType = "Dictionary";

            RepeatedField<NitricTask> tasks = new RepeatedField<NitricTask>();
            tasks.Add(taskToReturn);

            var queueReceieveResponse = new QueueReceiveResponse();
            queueReceieveResponse.Tasks.AddRange(tasks);

            Mock<Proto.Queue.v1.Queue.QueueClient> ec = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            ec.Setup(e => e.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueReceieveResponse)
                .Verifiable();

            var queueClient = new QueueClient.Builder()
                .Client(ec.Object)
                .Build();

            var response = queueClient.Receive("queue", 3);

            Assert.AreEqual("32", response[0].Event.RequestId);

            ec.Verify(t => t.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReceiveNoTasks()
        {
            Mock<Proto.Queue.v1.Queue.QueueClient> ec = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            ec.Setup(e => e.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueReceiveResponse())
                .Verifiable();

            var queueClient = new QueueClient.Builder()
                .Client(ec.Object)
                .Build();

            var response = queueClient.Receive("queue", 3);

            Assert.AreEqual(0, response.Count);

            ec.Verify(t => t.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
