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
using System.Text;
using System.Collections.Generic;
using System.Net;
using Nitric.Api.Http;
using System.Linq;
namespace Nitric.Test.Api.Http
{
    [TestClass]
    public class HttpResponseTest
    {
        [TestMethod]
        public void TestFullBuild()
        {
            var body = Encoding.UTF8.GetBytes("Hello World");

            var response = new HttpResponse
                .Builder()
                .Body(body)
                .Status(HttpStatusCode.Moved)
                .Headers(new Dictionary<string, List<string>>())
                .Build();

            Assert.AreEqual(body, response.Body);
            Assert.AreEqual(301, (int)response.Status);
            Assert.IsNotNull(response.Headers);

        }
        [TestMethod]
        public void TestDefaults()
        {
            var response = new HttpResponse
                .Builder()
                .Build();

            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(200, (int)response.Status);
            Assert.IsNotNull(response.Headers);
        }
        [TestMethod]
        public void TestHeaders()
        {
            var headers = new Dictionary<string, List<string>>();
            headers.Add("Content-length", new List<string>() { "1024" });
            headers.Add("Accept-Charset", new List<string>() { "ISO-8859-1" });

            var response = new HttpResponse
                    .Builder()
                    .Headers(headers)
                    .Build();

            Assert.IsNotNull(response.Headers);

            Assert.IsNotNull(response.Headers["Content-length"]);
            Assert.AreEqual("1024", response.Headers["Content-length"][0]);

            Assert.IsNotNull(response.Headers["Accept-Charset"]);
            Assert.AreEqual("ISO-8859-1", response.Headers["Accept-Charset"][0]);

            response = new HttpResponse
                    .Builder()
                    .Header("name", "value")
                    .Build();

            Assert.IsNotNull(response.Headers);
            Assert.AreEqual("value", response.Headers["name"][0]);
        }

        [TestMethod]
        public void TestBodyText()
        {
            var body = "Hello World";

            var response = new HttpResponse
                    .Builder()
                    .Status(HttpStatusCode.OK)
                    .Body(body)
                    .Build();

            Assert.AreEqual(200, (int)response.Status);
            Assert.IsNotNull(response.Headers);
            Assert.AreEqual(1, response.Headers.Count); //Default adds the Content Type
            Assert.IsNotNull(response.Body);
            Assert.AreEqual(body, response.BodyText);
            Assert.AreEqual(11, response.Body.Length);
        }
        [TestMethod]
        public void TestToString()
        {
            var response = new HttpResponse
                .Builder()
                .Build();

            Assert.AreEqual("HttpResponse[status=200, headers={}, body.length=0]",
                response.ToString());
        }
        [TestMethod]
        public void TestBuild()
        {
            var response = new HttpResponse
                .Builder()
                .Build(HttpStatusCode.OK);

            Assert.AreEqual(200, (int)response.Status);

            response = new HttpResponse
                .Builder()
                .Build(HttpStatusCode.OK, "Hello World");

            Assert.AreEqual(200, (int)response.Status);
            Assert.AreEqual("Hello World", response.BodyText);

            response = new HttpResponse
                .Builder()
                .Build("Hello World");

            Assert.AreEqual("Hello World", response.BodyText);
        }
    }
}
