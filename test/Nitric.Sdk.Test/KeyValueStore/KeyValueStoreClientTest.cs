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
    internal class FakeAsyncStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly List<T> results;
        private int index;

        public FakeAsyncStreamReader(List<T> results)
        {
            index = 0;
            this.results = results;
        }

        public T Current => results[index];

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (index == results.Count - 1)
            {
                return Task.FromResult(false);
            }

            index += 1;

            return Task.FromResult(true);
        }
    }


    public class TestProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Addresses { get; set; }
    }

    public class KeyValueStoreClientTest
    {
        [Fact]
        public void TestBuildKeyValueClient()
        {
            var storage = new KeyValueStoreClient();
            Assert.NotNull(storage);
        }

        [Fact]
        public void TestBuildKeyValueStoreWithName()
        {
            var store = new KeyValueStoreClient().KV<TestProfile>("test-store");
            Assert.NotNull(store);
            Assert.Equal("test-store", store.Name);
        }

        [Fact]
        public void TestBuildKeyValueStoreWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new KeyValueStoreClient().KV<TestProfile>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new KeyValueStoreClient().KV<TestProfile>(null)
            );
        }

        [Fact]
        public void TestSetToKeyValueStore()
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

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SetValue(It.IsAny<KvStoreSetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new KvStoreSetValueResponse())
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            kv.Set("test-key", testProfile);

            gc.Verify(
                t => t.SetValue(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestSetNullToKeyValueStore()
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
                e.SetValue(It.IsAny<KvStoreSetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new KvStoreSetValueResponse())
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            kv.Set("test-key", null);

            gc.Verify(
                t => t.SetValue(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestSetToKeyValueStoreWithError()
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

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.SetValue(It.IsAny<KvStoreSetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            try
            {
                kv.Set("test-key", testProfile);
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.SetValue(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSetToKeyValueStoreAsync()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            await kv.SetAsync("test-key", testProfile);

            gc.Verify(
                t => t.SetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSetNullToKeyValueStoreAsync()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            await kv.SetAsync("test-key", null);

            gc.Verify(
                t => t.SetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSetNullToKeyValueStoreAsyncWithError()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            try
            {
                await kv.SetAsync("test-key", null);
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
        public void TestGetToKeyValueStore()
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
                e.GetValue(It.IsAny<KvStoreGetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(resp)
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            var profile = kv.Get("test-key");

            Assert.Equal("John Smith", profile.Name);
            Assert.Equal(21, profile.Age);
            Assert.Equal("123 address street", profile.Addresses[0]);

            gc.Verify(
                t => t.GetValue(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestGetToKeyValueStoreWithError()
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
                e.GetValue(It.IsAny<KvStoreGetValueRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            try
            {
                kv.Get("test-key");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.GetValue(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestGetToKeyValueStoreAsync()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            var profile = await kv.GetAsync("test-key");

            Assert.Equal("John Smith", profile.Name);
            Assert.Equal(21, profile.Age);
            Assert.Equal("123 address street", profile.Addresses[0]);

            gc.Verify(
                t => t.GetValueAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestGetToKeyValueStoreAsyncWithError()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            try
            {
                await kv.GetAsync("test-key");
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
        public void TestDeleteKeyValuePair()
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
                e.DeleteKey(It.IsAny<KvStoreDeleteKeyRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new KvStoreDeleteKeyResponse())
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            kv.Delete("test-key");

            gc.Verify(
                t => t.DeleteKey(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestDeleteKeyValuePairWithError()
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
                e.DeleteKey(It.IsAny<KvStoreDeleteKeyRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key value store does not exist")))
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            try
            {
                kv.Delete("test-key");
                Assert.Fail();
            }
            catch (NitricException e)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified key value store does not exist\")",
                    e.Message);
            }

            gc.Verify(
                t => t.DeleteKey(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteKeyValuePairAsync()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            await kv.DeleteAsync("test-key");

            gc.Verify(
                t => t.DeleteKeyAsync(request, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteKeyValuePairAsyncWithError()
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

            try
            {
                await kv.DeleteAsync("test-key");
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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

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

            var kv = new KeyValueStoreClient(gc.Object).KV<TestProfile>("test-store");

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

