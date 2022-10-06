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
using Xunit;
using Nitric.Api.Http;
using System.Collections.Generic;
using System.Linq;
namespace Nitric.Test.Api.Http
{
    public class HttpRequestTest
    {
        [Fact]
        public void TestFullBuild()
        {
            var request = new HttpRequest
                .Builder()
                .Body("Hello World")
                .Method("GET")
                .Path("127.0.0.1:8080")
                .Query("POST")
                .Headers(new Dictionary<string, List<string>>())
                .Build();

            Assert.Equal("Hello World", request.BodyText);
            Assert.Equal("127.0.0.1:8080", request.Path);
            Assert.Equal("GET", request.Method);
            Assert.Equal("POST", request.Query);
            Assert.NotNull(request.Parameters);
            Assert.NotNull(request.Headers);
        }
        [Fact]
        public void TestBodyText()
        {
            var request = new HttpRequest
                .Builder()
                .Body("Hello World")
                .Build();

            Assert.Equal("Hello World", request.BodyText);
        }
        [Fact]
        public void TestHeaders()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
            headers.Add("Content-length", new List<string>() { "1024"});
            headers.Add("Accept-Charset", new List<string>() { "ISO-8859-1" });

            var request = new HttpRequest
                    .Builder()
                    .Headers(headers)
                    .Build();

            Assert.NotNull(request.Headers);
            Assert.NotNull(request.Headers["Content-length"]);
            Assert.Equal("1024", request.Headers["Content-length"][0]);

            Assert.NotNull(request.Headers["Accept-Charset"]);
            Assert.Equal("ISO-8859-1", request.Headers["Accept-Charset"][0]);

            //Assert.ThrowsException<ArgumentNullException>(() => request.Headers.Add("Accept-Charset", "utf-16".Split(',').ToList()), "Cant modify headers");
        }
        [Fact]
        public void TestDefaults()
        {
            var request = new HttpRequest
                .Builder()
                .Build();

            Assert.Equal(null, request.Body);
            Assert.Equal("", request.Path);
            Assert.Equal("", request.Method);
            Assert.Equal("", request.Query);
            Assert.NotNull(request.Headers);
            Assert.NotNull(request.Parameters);
        }
    }
}
