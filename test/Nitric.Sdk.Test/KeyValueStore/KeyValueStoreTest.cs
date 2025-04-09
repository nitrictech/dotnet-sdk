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
using Grpc.Core;
using Moq;
using Xunit;
using GrpcClient = Nitric.Proto.KvStore.v1.KvStore.KvStoreClient;
using Nitric.Sdk.KeyValueStore;
using Nitric.Proto.KvStore.v1;
using System.Threading;
using System.Threading.Tasks;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Test.KeyValueStore
{
    public class TestProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Addresses { get; set; }
    }

    public class KeyValueStoreClientTest
    {
        [Fact]
        public void TestBuildKeyValueStoreWithName()
        {
            var store = new KeyValueStore<TestProfile>("test-store");
            Assert.NotNull(store);
            Assert.Equal("test-store", store.Name);
        }

        [Fact]
        public void TestBuildKeyValueStoreWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new KeyValueStore<TestProfile>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new KeyValueStore<TestProfile>(null)
            );
        }

        [Fact]
        public void TestKeyValueStoreToString()
        {
            var job = new KeyValueStore<TestProfile>("test-store");

            Assert.Equal("KeyValueStore`1[name=test-store]", job.ToString());
        }

        [Fact]
        public async void TestSetToKeyValueStore()
        {
            var testProfile = new TestProfile
            {
                Name = "John Smith",
                Age = 21,
                Addresses = new List<string> { "123 address street" }
            };

            var payload = Sdk.Common.Struct.FromJsonSerializable(testProfile);

            var request = new KvStoreSetValueRequest
            {
                Content = payload,
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            var resp = new KvStoreSetValueResponse();

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SetValueAsync(It.IsAny<KvStoreSetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<KvStoreSetValueResponse>(Task.FromResult(resp), null, null, null, null, null))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            await kv.Set("test-key", testProfile);

            gc.Verify(
                t => t.SetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSetNullToKeyValueStore()
        {
            var request = new KvStoreSetValueRequest
            {
                Content = null,
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            var resp = new KvStoreSetValueResponse();

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SetValueAsync(It.IsAny<KvStoreSetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<KvStoreSetValueResponse>(Task.FromResult(resp), null, null, null, null, null))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            await kv.Set("test-key", null);

            gc.Verify(
                t => t.SetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSetNullToKeyValueStoreWithError()
        {
            var request = new KvStoreSetValueRequest
            {
                Content = null,
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SetValueAsync(It.IsAny<KvStoreSetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            try
            {
                await kv.Set("test-key", null);
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.SetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestGetToKeyValueStore()
        {
            var testProfile = new TestProfile
            {
                Name = "John Smith",
                Age = 21,
                Addresses = new List<string> { "123 address street" }
            };

            var payload = Sdk.Common.Struct.FromJsonSerializable(testProfile);

            var request = new KvStoreGetValueRequest
            {
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            var resp = new KvStoreGetValueResponse
            {
                Value = new Value
                {
                    Content = payload,
                    Ref = new ValueRef
                    {
                        Key = "test-key",
                        Store = "test-store"
                    }
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.GetValueAsync(It.IsAny<KvStoreGetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<KvStoreGetValueResponse>(Task.FromResult(resp), null, null, null, null, null))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            var profile = await kv.Get("test-key");

            Assert.Equal("John Smith", profile.Name);
            Assert.Equal(21, profile.Age);
            Assert.Equal("123 address street", profile.Addresses[0]);

            gc.Verify(
                t => t.GetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestGetToKeyValueStoreWithError()
        {
            var testProfile = new TestProfile
            {
                Name = "John Smith",
                Age = 21,
                Addresses = new List<string> { "123 address street" }
            };

            var payload = Sdk.Common.Struct.FromJsonSerializable(testProfile);

            var request = new KvStoreGetValueRequest
            {
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.GetValueAsync(It.IsAny<KvStoreGetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            try
            {
                await kv.Get("test-key");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.GetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteKeyValuePair()
        {
            var request = new KvStoreDeleteKeyRequest
            {
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            var resp = new KvStoreDeleteKeyResponse();

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.DeleteKeyAsync(It.IsAny<KvStoreDeleteKeyRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<KvStoreDeleteKeyResponse>(Task.FromResult(resp), null, null, null, null, null))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            await kv.Delete("test-key");

            gc.Verify(
                t => t.DeleteKeyAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteKeyValuePairWithError()
        {
            var request = new KvStoreDeleteKeyRequest
            {
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.DeleteKeyAsync(It.IsAny<KvStoreDeleteKeyRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            try
            {
                await kv.Delete("test-key");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }


            gc.Verify(
                t => t.DeleteKeyAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestGetListOfKeysWithNoPrefix()
        {
            var request = new KvStoreScanKeysRequest
            {
                Prefix = "",
                Store = new Store
                {
                    Name = "test-store",
                },
            };

            var responseStream = new FakeAsyncStreamReader<KvStoreScanKeysResponse>(
                new List<KvStoreScanKeysResponse>
                {
                    new KvStoreScanKeysResponse { Key = "key-1" },
                    new KvStoreScanKeysResponse { Key = "key-2" },
                    new KvStoreScanKeysResponse { Key = "key-3" },
                }
            );

            var resp = new AsyncServerStreamingCall<KvStoreScanKeysResponse>(responseStream, null, null, null, null);

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.ScanKeys(It.IsAny<KvStoreScanKeysRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(resp)
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            var keys = kv.Keys();

            gc.Verify(
                t => t.ScanKeys(request, null, null, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal("key-1", keys.Current);
            await keys.MoveNext();
            Assert.Equal("key-2", keys.Current);
            await keys.MoveNext(CancellationToken.None);
            Assert.Equal("key-3", keys.Current);
            var movedForward = await keys.MoveNext();
            Assert.False(movedForward);
        }

        [Fact]
        public async void TestGetListOfKeysWithPrefix()
        {
            var request = new KvStoreScanKeysRequest
            {
                Prefix = "key-",
                Store = new Store
                {
                    Name = "test-store",
                },
            };

            var responseStream = new FakeAsyncStreamReader<KvStoreScanKeysResponse>(
                new List<KvStoreScanKeysResponse>
                {
                    new KvStoreScanKeysResponse { Key = "key-1" },
                    new KvStoreScanKeysResponse { Key = "key-2" },
                    new KvStoreScanKeysResponse { Key = "key-3" },
                }
            );

            var resp = new AsyncServerStreamingCall<KvStoreScanKeysResponse>(responseStream, null, null, null, null);

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.ScanKeys(It.IsAny<KvStoreScanKeysRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(resp)
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            var keys = kv.Keys("key-");

            gc.Verify(
                t => t.ScanKeys(request, null, null, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal("key-1", keys.Current);
            await keys.MoveNext();
            Assert.Equal("key-2", keys.Current);
            await keys.MoveNext(CancellationToken.None);
            Assert.Equal("key-3", keys.Current);
            var movedForward = await keys.MoveNext();
            Assert.False(movedForward);
        }

        [Fact]
        public void TestGetListOfKeysWithError()
        {
            var request = new KvStoreScanKeysRequest
            {
                Prefix = "",
                Store = new Store
                {
                    Name = "test-store",
                },
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.ScanKeys(It.IsAny<KvStoreScanKeysRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStore<TestProfile>("test-store", gc.Object);

            try
            {
                kv.Keys();
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.ScanKeys(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

