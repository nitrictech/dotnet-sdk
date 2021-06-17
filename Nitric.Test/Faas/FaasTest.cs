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
using System;
using System.Text;
using System.Net.Http;
using System.Threading;
using Nitric.Faas;

namespace Nitric.Test.Faas
{
    //[TestClass]
    public class FaasTest
    {
        [TestMethod]
        public void TestDoubleStart()
        {
            Nitric.Faas.Faas faas = Nitric.Faas.Faas.NewBuilder()
            .Function(new HelloWorld())
            .Build();
            Thread faasThread = new Thread(() => faas.Start());
            faasThread.Start();
            Assert.ThrowsException<ArgumentException>(() => faas.Start());
            faas.Listener.Close();
        }
        [TestMethod]
        public void TestCall()
        {
            Nitric.Faas.Faas faas = Nitric.Faas.Faas.NewBuilder()
                .Function(new HelloWorld())
                .Build();
            Thread faasThread = new Thread(() => faas.Start());
            faasThread.Start();
            var client = new HttpClient();
            var result = client.GetAsync(string.Format("http://{0}:{1}/", "127.0.0.1", 8080));
            Assert.AreEqual("Hello World", result.Result);
        }
    }
    public class HelloWorld : INitricFunction
    {
        public int requestNum = 0;
        public Trigger[] triggers { get; private set; }
        public Response Handle(Trigger request)
        {
            triggers[requestNum++] = request;
            

            return request.DefaultResponse(Encoding.UTF8.GetBytes("Hello World"));
        }
    }
}
