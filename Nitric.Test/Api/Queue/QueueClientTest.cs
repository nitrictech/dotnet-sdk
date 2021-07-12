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
using Nitric.Proto.Queue.v1;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using Moq;
using Google.Protobuf.WellKnownTypes;
using Nitric.Api.Queue;

namespace Nitric.Test.Api.QueueClient
{
    [TestClass]
    public class QueueClientTest
    {
        [TestMethod]
        public void TestBuildQueues()
        {
            var queues = new Queues();
            Assert.IsNotNull(queues);
        }
        [TestMethod]
        public void TestBuildQueueWithName()
        {
            var queue = new Queues().Queue("test-queue");
            Assert.IsNotNull(queue);
            Assert.AreEqual("test-queue", queue.Name);
        }
        public void TestBuildQueueWithoutName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Queues().Queue("")
            );
            Assert.ThrowsException<ArgumentNullException>(
                () => new Queues().Queue(null)
            );
        }
        [TestMethod]
        public void TestSendBatchWithFailedTasks()
        {
            //Setting up failed tasks to then return later
            NitricTask failedTaskTask = new NitricTask();
            failedTaskTask.Id = "0";
            failedTaskTask.LeaseId = "1";
            failedTaskTask.Payload = Nitric.Api.Common.Util.ObjToStruct(new Dictionary<string, string>());
            failedTaskTask.PayloadType = "Dictionary";

            Proto.Queue.v1.FailedTask failedTask = new Proto.Queue.v1.FailedTask();
            failedTask.Message = "I am a failed task... I failed my task";
            failedTask.Task = failedTaskTask;

            List<Proto.Queue.v1.FailedTask> failedTasks = new List<Proto.Queue.v1.FailedTask>();
            failedTasks.Add(failedTask);

            var queueBatchResponse = new QueueSendBatchResponse();
            queueBatchResponse.FailedTasks.AddRange(failedTasks);

            Mock<Proto.Queue.v1.Queue.QueueClient> qc = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            qc.Setup(e => e.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueBatchResponse)
                .Verifiable();

            var queue = new Queues(qc.Object).Queue("test-queue");

            var response = queue.SendBatch(new List<Task>());

            Assert.AreEqual("I am a failed task... I failed my task", response.getFailedTasks()[0].Message);

            qc.Verify(t => t.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSendBatchWithNoFailedTasks()
        {
            Mock<Proto.Queue.v1.Queue.QueueClient> qc = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            qc.Setup(e => e.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueSendBatchResponse())
                .Verifiable();

            var queue = new Queues(qc.Object).Queue("test-queue");

            var response = queue.SendBatch(new List<Task>());

            Assert.AreEqual(0, response.getFailedTasks().Count);

            qc.Verify(t => t.SendBatch(It.IsAny<QueueSendBatchRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReceiveTasks()
        {
            NitricTask taskToReturn = new NitricTask();
            taskToReturn.Id = "32";
            taskToReturn.LeaseId = "1";
            taskToReturn.Payload = Nitric.Api.Common.Util.ObjToStruct(new Dictionary<string, string>());
            taskToReturn.PayloadType = "Dictionary";

            RepeatedField<NitricTask> tasks = new RepeatedField<NitricTask>();
            tasks.Add(taskToReturn);

            var queueReceieveResponse = new QueueReceiveResponse();
            queueReceieveResponse.Tasks.AddRange(tasks);

            Mock<Proto.Queue.v1.Queue.QueueClient> qc = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            qc.Setup(e => e.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queueReceieveResponse)
                .Verifiable();

            var queue = new Queues(qc.Object).Queue("test-queue");

            var response = queue.Receive(3);

            Assert.AreEqual("32", response[0].ID);

            qc.Verify(t => t.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReceiveNoTasks()
        {
            Mock<Proto.Queue.v1.Queue.QueueClient> qc = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            qc.Setup(e => e.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new QueueReceiveResponse())
                .Verifiable();

            var queue = new Queues(qc.Object).Queue("test-queue");

            var response = queue.Receive(3);

            Assert.AreEqual(0, response.Count);

            qc.Verify(t => t.Receive(It.IsAny<QueueReceiveRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSend()
        {
            Mock<Proto.Queue.v1.Queue.QueueClient> qc = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            qc.Setup(e => e.Send(It.IsAny<QueueSendRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Verifiable();

            var queue = new Queues(qc.Object).Queue("test-queue");

            queue.Send(new Task.Builder()
                .LeaseID("leaseId")
                .Id("0")
                .Payload(new Struct())
                .PayloadType("JSON")
                .Build()
            );

            qc.Verify(t => t.Send(It.IsAny<QueueSendRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestComplete()
        {
            Mock<Proto.Queue.v1.Queue.QueueClient> qc = new Mock<Proto.Queue.v1.Queue.QueueClient>();
            qc.Setup(e => e.Complete(It.IsAny<QueueCompleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Verifiable();

            var task = new Task.Builder()
                .Queue(new Queues(qc.Object).Queue("test-queue"))
                .Build();

            task.Complete();

            qc.Verify(t => t.Complete(It.IsAny<QueueCompleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
