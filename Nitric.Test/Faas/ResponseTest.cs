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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Nitric.Faas;

namespace Nitric.Test.Faas
{
    [TestClass]
    public class ResponseTest
    {
        [TestMethod]
        public void TestHttpToGrpc()
        {
            var ctx = new HttpResponseContext()
                .SetStatus(200)
                .AddHeader("x-nitric-testing", "test");
            
            var response = new Response(
              Encoding.UTF8.GetBytes("Hello World"),
              ctx
            );

            var triggerResponse = response.ToGrpcTriggerResponse();

            Assert.AreEqual(triggerResponse.Data.ToStringUtf8(), "Hello World");
            Assert.IsNotNull(triggerResponse.Http);
            Assert.AreEqual(triggerResponse.Http.Status, 200);
            Assert.AreEqual(triggerResponse.Http.Headers["x-nitric-testing"], "test");
        }

        public void TestTopicToGrpc()
        {
            var ctx = new TopicResponseContext()
                .SetSuccess(true);
            
            var response = new Response(
              Encoding.UTF8.GetBytes("Hello World"),
              ctx
            );

            var triggerResponse = response.ToGrpcTriggerResponse();

            Assert.AreEqual(triggerResponse.Data.ToStringUtf8(), "Hello World");
            Assert.IsNotNull(triggerResponse.Topic);
            Assert.AreEqual(triggerResponse.Topic.Success, true);
        }
    }
}