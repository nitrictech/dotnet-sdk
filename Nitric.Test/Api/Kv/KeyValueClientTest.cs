using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Nitric.Api.KeyValue;
using Moq;
using Nitric.Proto.KeyValue.v1;
namespace Nitric.Test.Api.Kv
{
    [TestClass]
    public class KeyValueClientTest
    {
        static readonly string KnownKey = "john.smith@gmail.com";
        static readonly Dictionary<string, object> KnownDict = new Dictionary<string, object>
        { {"name", "John Smith" } };
        static readonly Struct KnownStruct = Nitric.Api.Common.Util.ObjectToStruct(KnownDict);

        [TestMethod]
        public void TestBuild()
        {
            var client = new KeyValueClient
                .Builder()
                .Collection("customers")
                .Type(KnownDict.GetType())
                .Build();

            Assert.IsNotNull(client);
            Assert.AreEqual("customers", client.Collection);

            try
            {
                new KeyValueClient.Builder().Build(null, KnownDict.GetType());
                Assert.IsTrue(false);
            } catch (ArgumentNullException ane) {
                    Assert.AreEqual("Value cannot be null. (Parameter 'collection')", ane.Message);
            }

            try
            {
                new KeyValueClient.Builder().Build("collection", null);
                Assert.IsTrue(false);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'type')", ane.Message);
            }
        }

        [TestMethod]
        public void TestGet()
        {
            var request = new KeyValueGetRequest { Collection = "customers", Key = KnownKey};

            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>()
            { CallBase = true };
            ec.Setup(e => e.Get(request, It.IsAny<Grpc.Core.CallOptions>()))
                .Returns(new KeyValueGetResponse());

            ec.Verify(t => t.Get(request, It.IsAny<Grpc.Core.CallOptions>()), Times.Once);
        }

        [TestMethod]
        public void TestPut()
        {
            var request = new KeyValuePutRequest { Collection = "customers", Key = KnownKey, Value = KnownStruct };

            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>()
            { CallBase = true };
            ec.Setup(e => e.Put(request, It.IsAny<Grpc.Core.CallOptions>()))
                .Returns(new KeyValuePutResponse());

            ec.Verify(t => t.Put(request, It.IsAny<Grpc.Core.CallOptions>()), Times.Once);
        }

        [TestMethod]
        public void TestDelete()
        {
            var request = new KeyValueDeleteRequest { Collection = "customers", Key = KnownKey };

            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>()
            { CallBase = true };
            ec.Setup(e => e.Delete(request, It.IsAny<Grpc.Core.CallOptions>()))
                .Returns(new KeyValueDeleteResponse());

            ec.Verify(t => t.Delete(request, It.IsAny<Grpc.Core.CallOptions>()), Times.Once);
        }
    }
}
