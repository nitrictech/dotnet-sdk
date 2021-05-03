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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Proto.Storage.v1;
using Nitric.Api.Storage;
using Moq;
using Grpc.Core;

namespace Nitric.Test.Api.Storage
{
    [TestClass]
    public class StorageClientTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var storage = new StorageClient
                .Builder()
                .BucketName("bucket")
                .Build();

            Assert.AreEqual("bucket", storage.BucketName);
            Assert.IsNotNull(storage);

            try
            {
                storage = new StorageClient
                    .Builder()
                    .Build();
                Assert.IsTrue(false);
            } catch(ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'bucketName')", ane.Message);
            }
        }
        [TestMethod]
        public void TestWrite()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "bucket",
                Key = "key",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            var storageClient = new StorageClient.Builder()
                .Client(ec.Object)
                .BucketName("Customers")
                .Build();

            storageClient.Write("john smith", System.Text.Encoding.UTF8.GetBytes("Hello World"));

            ec.Verify(t => t.Write(It.IsAny<StorageWriteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReadExistingKey()
        {
            var storageResponse = new StorageReadResponse();
            storageResponse.Body = Google.Protobuf.ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(storageResponse)
                .Verifiable();

            var storageClient = new StorageClient.Builder()
                .Client(ec.Object)
                .BucketName("Customers")
                .Build();

            var response = storageClient.Read("john smith");

            Assert.AreEqual("Hello World", System.Text.Encoding.UTF8.GetString(response));

            ec.Verify(t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReadNonExistingKey()
        {
            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var storageClient = new StorageClient.Builder()
                .Client(ec.Object)
                .BucketName("Customers")
                .Build();

            try
            {
                storageClient.Read("john smith");
                Assert.IsTrue(false);
            } catch (RpcException re) {
                Assert.AreEqual("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")", re.Message);
            }

            ec.Verify(t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestDeleteExistingKey()
        {
            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageDeleteResponse())
                .Verifiable();

            var storageClient = new StorageClient.Builder()
                .Client(ec.Object)
                .BucketName("Customers")
                .Build();

            storageClient.Delete("john smith");

            ec.Verify(t => t.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestDeleteNonExistinghKey()
        {
            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var storageClient = new StorageClient.Builder()
                .Client(ec.Object)
                .BucketName("Customers")
                .Build();

            try
            {
                storageClient.Delete("john smith");
                Assert.IsTrue(false);
            } catch (RpcException re) {
                Assert.AreEqual("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")", re.Message);
            }
            ec.Verify(t => t.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
