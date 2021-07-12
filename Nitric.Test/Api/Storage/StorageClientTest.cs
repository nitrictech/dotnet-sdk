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
using Grpc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nitric.Proto.Storage.v1;
using Storage = Nitric.Api.Storage.Storage;

namespace Nitric.Test.Api.StorageClient
{
    [TestClass]
    public class StorageClientTest
    {
        [TestMethod]
        public void TestBuildStorage()
        {
            var storage = new Storage();
            Assert.IsNotNull(storage);
        }
        [TestMethod]
        public void TestBuildBucketWithName()
        {
            var bucket = new Storage().Bucket("test-bucket");
            Assert.IsNotNull(bucket);
            Assert.AreEqual("test-bucket", bucket.Name);
        }
        [TestMethod]
        public void TestBuildBucketWithoutName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Storage().Bucket("")
            );
            Assert.ThrowsException<ArgumentNullException>(
                () => new Storage().Bucket(null)
            );
        }
        [TestMethod]
        public void TestBuildFileWithName()
        {
            var file = new Storage().Bucket("test-bucket").File("test-file");
            Assert.IsNotNull(file);
        }
        [TestMethod]
        public void TestBuildFileWithoutName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Storage().Bucket("test-bucket").File("")
            );
            Assert.ThrowsException<ArgumentNullException>(
                () => new Storage().Bucket("test-bucket").File(null)
            );
        } 
        [TestMethod]
        public void TestWrite()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<Proto.Storage.v1.Storage.StorageClient> bc = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            var file = new Storage(bc.Object).Bucket("test-bucket").File("test-file");

            file.Write(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            bc.Verify(t => t.Write(It.IsAny<StorageWriteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReadExistingKey()
        {
            var storageResponse = new StorageReadResponse();
            storageResponse.Body = Google.Protobuf.ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            Mock<Proto.Storage.v1.Storage.StorageClient> bc = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            bc.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(storageResponse)
                .Verifiable();

            var file = new Storage(bc.Object).Bucket("test-bucket").File("test-file");

            var response = file.Read();

            Assert.AreEqual("Hello World", System.Text.Encoding.UTF8.GetString(response));

            bc.Verify(t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestReadNonExistingKey()
        {
            Mock<Proto.Storage.v1.Storage.StorageClient> bc = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            bc.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Storage(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.Read();
                Assert.IsTrue(false);
            } catch (RpcException re) {
                Assert.AreEqual("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")", re.Message);
            }

            bc.Verify(t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestDeleteExistingKey()
        {
            Mock<Proto.Storage.v1.Storage.StorageClient> bc = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            bc.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageDeleteResponse())
                .Verifiable();

            var file = new Storage(bc.Object).Bucket("test-bucket").File("test-file");

            file.Delete();

            bc.Verify(t => t.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
