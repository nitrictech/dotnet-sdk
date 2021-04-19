﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Faas;
using System.Collections.Generic;
using System.Linq;
namespace Nitric.Test.Api.Faas
{
    [TestClass]
    public class NitricRequestTest
    {
        [TestMethod]
        public void TestFullBuild()
        {
            var request = new NitricRequest
                .Builder()
                .Body("Hello World")
                .Method("GET")
                .Path("127.0.0.1:8080")
                .Query("POST")
                .Headers(new Dictionary<string, List<string>>())
                .Build();

            Assert.AreEqual("Hello World", request.BodyText);
            Assert.AreEqual("127.0.0.1:8080", request.Path);
            Assert.IsNotNull(request.Parameters);
            Assert.IsNotNull(request.Headers);
        }
        [TestMethod]
        public void TestBodyText()
        {
            var request = new NitricRequest
                .Builder()
                .Body("Hello World")
                .Build();

            Assert.AreEqual("Hello World", request.BodyText);
        }
        [TestMethod]
        public void TestHeaders()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-length", "1024");
            headers.Add("Accept-Charset", "ISO-8859-1");

            var request = new NitricRequest
                    .Builder()
                    .Headers(headers)
                    .Build();

            Assert.IsNotNull(request.Headers);
            Assert.IsNotNull(request.Headers["Content-length"]);
            Assert.AreEqual("1024", request.Headers["Content-length"][0]);

            Assert.IsNotNull(request.Headers["Accept-Charset"]);
            Assert.AreEqual("ISO-8859-1", request.Headers["Accept-Charset"][0]);

            //Assert.ThrowsException<ArgumentNullException>(() => request.Headers.Add("Accept-Charset", "utf-16".Split(',').ToList()), "Cant modify headers");
        }
        [TestMethod]
        public void TestDefaults()
        {
            var request = new NitricRequest
                .Builder()
                .Build();

            Assert.AreEqual(null, request.Body);
            Assert.AreEqual("", request.Path);
            Assert.IsNotNull(request.Headers);
            Assert.IsNotNull(request.Parameters);
        }
    }
}