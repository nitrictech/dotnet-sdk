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
using Moq;
using Nitric.Proto.Storage.v1;
using Xunit;

namespace Nitric.Sdk.Test.Api.Storage
{
    public class StorageClientTest
    {
        [Fact]
        public void TestBuildStorage()
        {
            var storage = new Sdk.Storage.Storage();
            Assert.NotNull(storage);
        }

        [Fact]
        public void TestBuildBucketWithName()
        {
            var bucket = new Sdk.Storage.Storage().Bucket("test-bucket");
            Assert.NotNull(bucket);
            Assert.Equal("test-bucket", bucket.Name);
        }

        [Fact]
        public void TestBuildBucketWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.Storage().Bucket("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.Storage().Bucket(null)
            );
        }

        [Fact]
        public void TestBuildFileWithName()
        {
            var file = new Sdk.Storage.Storage().Bucket("test-bucket").File("test-file");
            Assert.NotNull(file);
            Assert.Equal("test-file", file.Name);
        }

        [Fact]
        public void TestBuildFileWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.Storage().Bucket("test-bucket").File("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.Storage().Bucket("test-bucket").File(null)
            );
        }

        [Fact]
        public void TestWrite()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<StorageService.StorageServiceClient> bc = new Mock<StorageService.StorageServiceClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            var file = new Sdk.Storage.Storage(bc.Object).Bucket("test-bucket").File("test-file");

            file.Write(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            bc.Verify(
                t => t.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestWriteString()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<StorageService.StorageServiceClient> bc = new Mock<StorageService.StorageServiceClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            var file = new Sdk.Storage.Storage(bc.Object).Bucket("test-bucket").File("test-file");

            file.Write("Hello World");

            bc.Verify(
                t => t.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestReadExistingKey()
        {
            var storageResponse = new StorageReadResponse();
            storageResponse.Body =
                Google.Protobuf.ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            Mock<StorageService.StorageServiceClient> bc = new Mock<StorageService.StorageServiceClient>();
            bc.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(storageResponse)
                .Verifiable();

            var file = new Sdk.Storage.Storage(bc.Object).Bucket("test-bucket").File("test-file");

            var response = file.Read();

            Assert.Equal("Hello World", System.Text.Encoding.UTF8.GetString(response));

            bc.Verify(
                t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestReadNonExistingKey()
        {
            Mock<StorageService.StorageServiceClient> bc = new Mock<StorageService.StorageServiceClient>();
            bc.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.Storage(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.Read();
                Assert.True(false);
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestDeleteExistingKey()
        {
            Mock<StorageService.StorageServiceClient> bc = new Mock<StorageService.StorageServiceClient>();
            bc.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageDeleteResponse())
                .Verifiable();

            var file = new Sdk.Storage.Storage(bc.Object).Bucket("test-bucket").File("test-file");

            file.Delete();

            bc.Verify(
                t => t.Delete(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
