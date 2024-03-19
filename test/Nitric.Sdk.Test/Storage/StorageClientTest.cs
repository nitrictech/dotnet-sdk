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
using System.Threading.Tasks;
using Grpc.Core;
using Moq;
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Common;
using Xunit;
using GrpcClient = Nitric.Proto.Storage.v1.Storage.StorageClient;

namespace Nitric.Sdk.Test.Storage
{

    public class StorageClientTest
    {
        [Fact]
        public void TestBuildStorage()
        {
            var storage = new Sdk.Storage.StorageClient();
            Assert.NotNull(storage);
        }

        [Fact]
        public void TestBuildBucketWithName()
        {
            var bucket = new Sdk.Storage.StorageClient().Bucket("test-bucket");
            Assert.NotNull(bucket);
            Assert.Equal("test-bucket", bucket.Name);
        }

        [Fact]
        public void TestBuildBucketWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.StorageClient().Bucket("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.StorageClient().Bucket(null)
            );
        }

        [Fact]
        public void TestBuildFileWithName()
        {
            var file = new Sdk.Storage.StorageClient().Bucket("test-bucket").File("test-file");
            Assert.NotNull(file);
            Assert.Equal("test-file", file.Name);
        }

        [Fact]
        public void TestBuildFileWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.StorageClient().Bucket("test-bucket").File("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new Sdk.Storage.StorageClient().Bucket("test-bucket").File(null)
            );
        }

        [Fact]
        public void TestBucketToString()
        {
            var bucket = new Sdk.Storage.StorageClient().Bucket("test-bucket");
            Assert.Equal("Bucket[name=test-bucket]", bucket.ToString());
        }

        [Fact]
        public void TestFileToString()
        {
            var file = new Sdk.Storage.StorageClient().Bucket("test-bucket").File("test-file");
            Assert.Equal("File[name=test-file\nbucket=test-bucket]", file.ToString());
        }

        [Fact]
        public void TestListBlobsWithNoPrefix()
        {
            var request = new StorageListBlobsRequest
            {
                BucketName = "test-bucket",
                Prefix = "",
            };

            var blobs = new List<Blob>
            {
                new Blob { Key = "key-1" },
                new Blob { Key = "key-2" },
                new Blob { Key = "key-3" },
            };
            var response = new StorageListBlobsResponse();
            response.Blobs.AddRange(blobs);

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ListBlobs(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(response)
                .Verifiable();

            var files = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").Files();

            bc.Verify(
                t => t.ListBlobs(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("key-1", files[0].Name);
            Assert.Equal("key-2", files[1].Name);
            Assert.Equal("key-3", files[2].Name);
        }

        [Fact]
        public void TestListBlobsWithPrefix()
        {
            var request = new StorageListBlobsRequest
            {
                BucketName = "test-bucket",
                Prefix = "key-",
            };

            var blobs = new List<Blob>
            {
                new Blob { Key = "key-1" },
                new Blob { Key = "key-2" },
                new Blob { Key = "key-3" },
            };
            var response = new StorageListBlobsResponse();
            response.Blobs.AddRange(blobs);

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ListBlobs(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(response)
                .Verifiable();

            var files = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").Files("key-");

            bc.Verify(
                t => t.ListBlobs(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("key-1", files[0].Name);
            Assert.Equal("key-2", files[1].Name);
            Assert.Equal("key-3", files[2].Name);
        }

        [Fact]
        public void TestListBlobsToNonExistentBucket()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ListBlobs(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            try
            {
                new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").Files("key-1");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.ListBlobs(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestListBlobsWithNoPrefixAsync()
        {
            var request = new StorageListBlobsRequest
            {
                BucketName = "test-bucket",
                Prefix = "",
            };

            var blobs = new List<Blob>
            {
                new Blob { Key = "key-1" },
                new Blob { Key = "key-2" },
                new Blob { Key = "key-3" },
            };
            var response = new StorageListBlobsResponse();
            response.Blobs.AddRange(blobs);

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ListBlobsAsync(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StorageListBlobsResponse>(Task.FromResult(response), null, null, null, null))
                .Verifiable();

            var files = await new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").FilesAsync();

            bc.Verify(
                t => t.ListBlobsAsync(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("key-1", files[0].Name);
            Assert.Equal("key-2", files[1].Name);
            Assert.Equal("key-3", files[2].Name);
        }

        [Fact]
        public async void TestListBlobsWithPrefixAsync()
        {
            var request = new StorageListBlobsRequest
            {
                BucketName = "test-bucket",
                Prefix = "key-",
            };

            var blobs = new List<Blob>
            {
                new Blob { Key = "key-1" },
                new Blob { Key = "key-2" },
                new Blob { Key = "key-3" },
            };
            var response = new StorageListBlobsResponse();
            response.Blobs.AddRange(blobs);

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ListBlobsAsync(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StorageListBlobsResponse>(Task.FromResult(response), null, null, null, null))
                .Verifiable();

            var files = await new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").FilesAsync("key-");

            bc.Verify(
                t => t.ListBlobsAsync(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("key-1", files[0].Name);
            Assert.Equal("key-2", files[1].Name);
            Assert.Equal("key-3", files[2].Name);
        }

        [Fact]
        public async void TestListBlobsToNonExistentBucketAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ListBlobsAsync(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            try
            {
                await new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").FilesAsync();
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.ListBlobsAsync(It.IsAny<StorageListBlobsRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
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

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

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

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            file.Write("Hello World");

            bc.Verify(
                t => t.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestWriteToNonExistentBucket()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.Write("Hello World");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestWriteBytesToNonExistentBucket()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Hello World"))
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.Write(System.Text.Encoding.UTF8.GetBytes("Hello World"));
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.Write(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestWriteAsync()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StorageWriteResponse>(Task.FromResult(new StorageWriteResponse()), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            await file.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            bc.Verify(
                t => t.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestWriteBytesToNonExistentBucketAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Hello World"));
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestWriteToNonExistentBucketAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.WriteAsync("Hello World");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestWriteStringAsync()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StorageWriteResponse>(Task.FromResult(new StorageWriteResponse()), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            await file.WriteAsync("Hello World");

            bc.Verify(
                t => t.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestWriteStringToNonExistentBucketAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified bucket does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.WriteAsync("Hello World");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified bucket does not exist\")",
                    e.Message);
            }

            bc.Verify(
                t => t.WriteAsync(It.IsAny<StorageWriteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestReadExistingKey()
        {
            var storageResponse = new StorageReadResponse();
            storageResponse.Body =
                Google.Protobuf.ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(storageResponse)
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var response = file.Read();

            Assert.Equal("Hello World", System.Text.Encoding.UTF8.GetString(response));

            bc.Verify(
                t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestReadNonExistingKey()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.Read();
                Assert.True(false);
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.Read(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void TestReadExistingKeyAsync()
        {
            var storageResponse = new StorageReadResponse();
            storageResponse.Body =
                Google.Protobuf.ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes("Hello World"));

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ReadAsync(It.IsAny<StorageReadRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StorageReadResponse>(Task.FromResult(storageResponse), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var response = await file.ReadAsync();

            Assert.Equal("Hello World", System.Text.Encoding.UTF8.GetString(response));

            bc.Verify(
                t => t.ReadAsync(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void TestReadNonExistingKeyAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.ReadAsync(It.IsAny<StorageReadRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.ReadAsync();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.ReadAsync(It.IsAny<StorageReadRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestDeleteExistingKey()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StorageDeleteResponse())
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            file.Delete();

            bc.Verify(
                t => t.Delete(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestDeleteNonExistingKey()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.Delete();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.Delete(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void TestDeleteExistingKeyAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.DeleteAsync(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StorageDeleteResponse>(Task.FromResult(new StorageDeleteResponse()), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            await file.DeleteAsync();

            bc.Verify(
                t => t.DeleteAsync(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteNonExistingKeyAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.DeleteAsync(It.IsAny<StorageDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.DeleteAsync();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.DeleteAsync(It.IsAny<StorageDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestGetUploadUrlWithDefaultExpiry()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 600 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StoragePreSignUrlResponse { Url = "https://example.com" })
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = file.GetUploadUrl();

            bc.Verify(
                t => t.PreSignUrl(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public void TestGetUploadUrlWithSpecificExpiry()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 300 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StoragePreSignUrlResponse { Url = "https://example.com" })
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = file.GetUploadUrl(300);

            bc.Verify(
                t => t.PreSignUrl(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public void TestGetUploadUrlNonExistingKey()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.GetUploadUrl();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void TestGetUploadUrlWithDefaultExpiryAsync()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 600 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StoragePreSignUrlResponse>(Task.FromResult(new StoragePreSignUrlResponse { Url = "https://example.com" }), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = await file.GetUploadUrlAsync();

            bc.Verify(
                t => t.PreSignUrlAsync(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public async void TestGetUploadUrlWithSpecificExpiryAsync()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 300 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StoragePreSignUrlResponse>(Task.FromResult(new StoragePreSignUrlResponse { Url = "https://example.com" }), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = await file.GetUploadUrlAsync(300);

            bc.Verify(
                t => t.PreSignUrlAsync(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public async void TestGetUploadUrlNonExistingKeyAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.GetUploadUrlAsync();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestGetDownloadUrlWithDefaultExpiry()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Read,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 600 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StoragePreSignUrlResponse { Url = "https://example.com" })
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = file.GetDownloadUrl();

            bc.Verify(
                t => t.PreSignUrl(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public void TestGetDownloadUrlWithSpecificExpiry()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Read,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 300 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new StoragePreSignUrlResponse { Url = "https://example.com" })
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = file.GetDownloadUrl(300);

            bc.Verify(
                t => t.PreSignUrl(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public void TestGetDownloadUrlNonExistingKey()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                file.GetDownloadUrl();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.PreSignUrl(It.IsAny<StoragePreSignUrlRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void TestGetDownloadUrlWithDefaultExpiryAsync()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Read,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 600 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StoragePreSignUrlResponse>(Task.FromResult(new StoragePreSignUrlResponse { Url = "https://example.com" }), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = await file.GetDownloadUrlAsync();

            bc.Verify(
                t => t.PreSignUrlAsync(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public async void TestGetDownloadUrlWithSpecificExpiryAsync()
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = "test-bucket",
                Key = "test-file",
                Operation = StoragePreSignUrlRequest.Types.Operation.Read,
                Expiry = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 300 },
            };

            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new AsyncUnaryCall<StoragePreSignUrlResponse>(Task.FromResult(new StoragePreSignUrlResponse { Url = "https://example.com" }), null, null, null, null))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            var url = await file.GetDownloadUrlAsync(300);

            bc.Verify(
                t => t.PreSignUrlAsync(request, null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            Assert.Equal("https://example.com", url);
        }

        [Fact]
        public async void TestGetDownloadUrlNonExistingKeyAsync()
        {
            Mock<GrpcClient> bc = new Mock<GrpcClient>();
            bc.Setup(e => e.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var file = new Sdk.Storage.StorageClient(bc.Object).Bucket("test-bucket").File("test-file");

            try
            {
                await file.GetDownloadUrlAsync();
                Assert.Fail();
            }
            catch (NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")",
                    ne.Message);
            }

            bc.Verify(
                t => t.PreSignUrlAsync(It.IsAny<StoragePreSignUrlRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }
    }
}