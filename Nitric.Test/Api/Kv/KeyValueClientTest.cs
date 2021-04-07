﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Nitric.Api.KeyValue;
using Moq;
using Nitric.Proto.KeyValue.v1;
using Grpc.Core;
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
            } catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'type')", ane.Message);
            }
        }

        [TestMethod]
        public void TestGetNonExistingKey()
        {
            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>();
            ec.Setup(e => e.Get(It.IsAny<KeyValueGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var kvClient = new KeyValueClient.Builder()
                .Collection("customers")
                .Type(typeof(string))
                .Client(ec.Object)
                .Build();

            try
            {
                kvClient.Get("key");
                Assert.IsTrue(false);
            } catch (RpcException re){
                Assert.AreEqual("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")", re.Message);
            }

            ec.Verify(t => t.Get(It.IsAny<KeyValueGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestGetExistingKey()
        {
            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>();
            ec.Setup(e => e.Get(It.IsAny<KeyValueGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValueGetResponse { Value = KnownStruct})
                .Verifiable();

            var kvClient = new KeyValueClient.Builder()
                .Collection("customers")
                .Type(typeof(string))
                .Client(ec.Object)
                .Build();

            var response = kvClient.Get("key");

            Assert.AreEqual(KnownStruct.ToString(), response);

            ec.Verify(t => t.Get(It.IsAny<KeyValueGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestPut()
        {
            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>();
            ec.Setup(e => e.Put(It.IsAny<KeyValuePutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValuePutResponse())
                .Verifiable();

            var kvClient = new KeyValueClient.Builder()
                .Collection("customers")
                .Type(typeof(string))
                .Client(ec.Object)
                .Build();

            kvClient.Put("customers", KnownStruct);

            ec.Verify(t => t.Put(It.IsAny<KeyValuePutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDeleteExistingKey()
        {
            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>();
            ec.Setup(e => e.Delete(It.IsAny<KeyValueDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new KeyValueDeleteResponse())
                .Verifiable();

            var kvClient = new KeyValueClient.Builder()
                .Collection("customers")
                .Type(typeof(string))
                .Client(ec.Object)
                .Build();

            kvClient.Delete("john smith");

            ec.Verify(t => t.Delete(It.IsAny<KeyValueDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestDeleteNonExistingKey()
        {
            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>();
            ec.Setup(e => e.Delete(It.IsAny<KeyValueDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var kvClient = new KeyValueClient.Builder()
                .Collection("customers")
                .Type(typeof(string))
                .Client(ec.Object)
                .Build();

            try
            {
                kvClient.Delete("key");
                Assert.IsTrue(false);
            } catch (RpcException re) {
                Assert.AreEqual("Status(StatusCode=\"NotFound\", Detail=\"The specified key does not exist\")", re.Message);
            }
            ec.Verify(t => t.Delete(It.IsAny<KeyValueDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
