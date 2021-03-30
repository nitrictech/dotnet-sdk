using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Faas;
using System.Collections.Generic;
using System.Linq;
namespace Nitric.Test
{
    [TestClass]
    public class NitricRequestTest
    {
        [TestMethod]
        public void TestFullBuild()
        {
            var request = NitricRequest
                .NewBuilder()
                .Body(System.Text.Encoding.UTF8.GetBytes("Hello World"))
                .Method("GET")
                .Path("127.0.0.1:8080")
                .Query("POST")
                .Headers(new Dictionary<string, List<string>>())
                .Build();

            Assert.AreEqual("Hello World", request.Body);
            Assert.AreEqual("127.0.0.1:8080", request.Path);
            Assert.IsNotNull(request.Parameters);
            Assert.IsNotNull(request.Headers);
        }
        [TestMethod]
        public void TestBodyText()
        {
            var request = NitricRequest
                .NewBuilder()
                .BodyText("Hello World")
                .Build();

            Assert.Equals("Hello World", System.Text.Encoding.UTF8.GetString(request.Body));
        }
        [TestMethod]
        public void TestHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Content-length", "1024");
            headers.Add("Accept-Charset", "ISO-8859-1");

            var request = NitricRequest
                    .NewBuilder()
                    .Headers(headers)
                    .Build();

            Assert.IsNotNull(request.Headers);

            Assert.IsNotNull(request.Headers["Content-length"]);
            Assert.AreEqual("1024", request.Headers["Content-length"]);

            Assert.IsNotNull(request.Headers["Accept-Charset"]);
            Assert.AreEqual("ISO-8859-1", request.Headers["Accept-Charset"]);


            try
            {
                Assert.IsTrue(request.Headers.TryAdd("Accept-Charset", "utf-16".Split(',').ToList()), "Cant modify headers");

            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void TestDefaults()
        {
            var request = NitricRequest
                .NewBuilder()
                .Build();

            Assert.AreEqual(null, request.Body);
            Assert.AreEqual("", request.Path);
            Assert.IsNotNull(request.Headers);
            Assert.IsNotNull(request.Parameters);
        }
    }
}
