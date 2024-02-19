using System;
using System.Collections.Generic;
using Grpc.Core;
using Moq;
using Xunit;
using GrpcClient = Nitric.Proto.KeyValue.v1.KeyValue.KeyValueClient;
using Nitric.Sdk.KeyValueStore;
using Nitric.Proto.KeyValue.v1;

namespace Nitric.Sdk.Test.Api.KeyValueStore
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
        public void TestBuildKeyValueClient()
        {
            var storage = new KeyValueStoreClient();
            Assert.NotNull(storage);
        }

        [Fact]
        public void TestBuildKeyValueStoreWithName()
        {
            var store = new KeyValueStoreClient().Store<TestProfile>("test-store");
            Assert.NotNull(store);
            Assert.Equal("test-store", store.Name);
        }

        [Fact]
        public void TestBuildKeyValueStoreWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new KeyValueStoreClient().Store<TestProfile>("")
            );
            Assert.Throws<ArgumentNullException>(
                () => new KeyValueStoreClient().Store<TestProfile>(null)
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

            var payload = Common.Struct.FromJsonSerializable(testProfile);

            var request = new KeyValueSetRequest
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
                e.Set(It.IsAny<KeyValueSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValueSetResponse())
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).Store<TestProfile>("test-store");

            kv.Set("test-key", testProfile);

            gc.Verify(
                t => t.Set(request, null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestSetNullToKeyValueStore()
        {
            var request = new KeyValueSetRequest
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
                e.Set(It.IsAny<KeyValueSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValueSetResponse())
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).Store<TestProfile>("test-store");

            kv.Set("test-key", null);

            gc.Verify(
                t => t.Set(request, null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
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

            var payload = Common.Struct.FromJsonSerializable(testProfile);

            var request = new KeyValueGetRequest
            {
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.Get(It.IsAny<KeyValueGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValueGetResponse { Value = new Value { Content = payload, Ref = new ValueRef { Key = "test-key", Store = "test-store" } } })
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).Store<TestProfile>("test-store");

            var profile = kv.Get("test-key");

            Assert.Equal("John Smith", profile.Name);
            Assert.Equal(21, profile.Age);
            Assert.Equal("123 address street", profile.Addresses[0]);

            gc.Verify(
                t => t.Get(request, null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestDeleteKeyValuePair()
        {
            var request = new KeyValueDeleteRequest
            {
                Ref = new ValueRef
                {
                    Key = "test-key",
                    Store = "test-store"
                }
            };

            Mock<GrpcClient> gc = new Mock<GrpcClient>();
            gc.Setup(e =>
                e.Delete(It.IsAny<KeyValueDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValueDeleteResponse())
                .Verifiable();

            var kv = new KeyValueStoreClient(gc.Object).Store<TestProfile>("test-store");

            kv.Delete("test-key");

            gc.Verify(
                t => t.Delete(request, null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}

