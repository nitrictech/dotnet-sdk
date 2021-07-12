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
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Nitric.Api.KeyValue;
using Moq;
using Nitric.Proto.KeyValue.v1;
using Grpc.Core;

namespace Nitric.Test.Api.KvClient
{
    [TestClass]
    public class KeyValueClientTest
    {
        static readonly Dictionary<string, object> KnownDict = new Dictionary<string, object>
        { {"name", "John Smith" } };
        static readonly Struct KnownStruct = Nitric.Api.Common.Util.ObjToStruct(KnownDict);

        [TestMethod]
        public void TestBuild()
        {
            var successfulClient = KeyValueClient<Dictionary<string, object>>
                .NewBuilder()
                .Collection("customers")
                .Build();

            Assert.IsNotNull(successfulClient);

            try
            {
                 var unsuccessfulClient = KeyValueClient<Dictionary<string, object>>
                    .NewBuilder()
                    .Build();
                Assert.IsTrue(false);
            } catch (ArgumentNullException ane) {
                Assert.AreEqual("Value cannot be null. (Parameter 'collection')", ane.Message);
            }
        }

        [TestMethod]
        public void TestGetNonExistingKey()
        {
            Mock<KeyValue.KeyValueClient> ec = new Mock<KeyValue.KeyValueClient>();
            ec.Setup(e => e.Get(It.IsAny<KeyValueGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified key does not exist")))
                .Verifiable();

            var kvClient = KeyValueClient<string>.NewBuilder()
                .Collection("customers")
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

            var kvClient = KeyValueClient<String>.NewBuilder()
                .Collection("customers")
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

            var kvClient = KeyValueClient<string>.NewBuilder()
                .Collection("customers")
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

            var kvClient = KeyValueClient<string>.NewBuilder()
                .Collection("customers")
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

            var kvClient = KeyValueClient<string>.NewBuilder()
                .Collection("customers")
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
