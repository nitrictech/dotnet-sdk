using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Http;
using System.Collections.Generic;
using System.Linq;
namespace Nitric.Test.Api.Http
{
    [TestClass]
    public class HttpRequestTest
    {
        [TestMethod]
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

            Assert.AreEqual("Hello World", request.BodyText);
            Assert.AreEqual("127.0.0.1:8080", request.Path);
            Assert.AreEqual("GET", request.Method);
            Assert.AreEqual("POST", request.Query);
            Assert.IsNotNull(request.Parameters);
            Assert.IsNotNull(request.Headers);
        }
        [TestMethod]
        public void TestBodyText()
        {
            var request = new HttpRequest
                .Builder()
                .Body("Hello World")
                .Build();

            Assert.AreEqual("Hello World", request.BodyText);
        }
        [TestMethod]
        public void TestHeaders()
        {
            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
            headers.Add("Content-length", new List<string>() { "1024"});
            headers.Add("Accept-Charset", new List<string>() { "ISO-8859-1" });

            var request = new HttpRequest
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
            var request = new HttpRequest
                .Builder()
                .Build();

            Assert.AreEqual(null, request.Body);
            Assert.AreEqual("", request.Path);
            Assert.AreEqual("", request.Method);
            Assert.AreEqual("", request.Query);
            Assert.IsNotNull(request.Headers);
            Assert.IsNotNull(request.Parameters);
        }
    }
}
