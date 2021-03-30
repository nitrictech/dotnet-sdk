using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Api.Faas;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;

namespace Nitric.Test
{
    [TestClass]
    public class NitricResponseTest
    {
        [TestMethod]
        public void TestFullBuild()
        {
            var body = Encoding.UTF8.GetBytes("Hello World");

            var response = NitricResponse
                .NewBuilder()
                .Body(body)
                .Status(HttpStatusCode.Moved)
                .Headers(new Dictionary<string, List<string>>())
                .Build();

            Assert.AreEqual(body, response.Body);
            Assert.AreEqual(301, (int)response.Status);
            Assert.AreEqual(new Dictionary<string, List<string>>(), response.Headers);

        }
        [TestMethod]
        public void TestDefaults()
        {
            var response = NitricResponse
                .NewBuilder()
                .Build();

            Assert.AreEqual(null, response.Body);
            Assert.AreEqual(200, (int)response.Status);
            Assert.IsNotNull(response.Headers);
        }
        [TestMethod]
        public void TestHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Content-length", "1024");
            headers.Add("Accept-Charset", "ISO-8859-1");

            var response = NitricResponse
                    .NewBuilder()
                    .Headers(headers)
                    .Build();

            Assert.IsNotNull(response.Headers);

            Assert.IsNotNull(response.Headers["Content-length"]);
            Assert.AreEqual("1024", response.Headers["Content-length"]);

            Assert.IsNotNull(response.Headers["Accept-Charset"]);
            Assert.AreEqual("ISO-8859-1", response.Headers["Accept-Charset"]);


            try
            {
                Assert.IsTrue(response.Headers.TryAdd("Accept-Charset", "utf-16".Split(',').ToList()), "Cant modify headers");

            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }

            response = NitricResponse
                    .NewBuilder()
                    .Header("name", "value")
                    .Build();

            Assert.IsNotNull(response.Headers);
            Assert.Equals("value", response.Headers["name"]);
        }

        [TestMethod]
        public void TestBodyText()
        {
            var body = "hello world";

            var response = NitricResponse
                    .NewBuilder()
                    .Status(HttpStatusCode.OK)
                    .BodyText(body)
                    .Build();

            Assert.AreNotEqual(200, (int)response.Status);
            Assert.IsNotNull(response.Headers);
            Assert.AreEqual(0, response.Headers.Count);
            Assert.IsNotNull(response.Body);
            Assert.AreEqual(body, Encoding.UTF8.GetString(response.Body));
            Assert.AreEqual(11, response.Body.Length);
            Assert.AreEqual(body, Encoding.UTF8.GetString(response.Body));
        }
        [TestMethod]
        public void TestToString()
        {
            var response = NitricResponse
                .NewBuilder()
                .Build();

            Assert.Equals("NitricResponse[status=0, headers={}, body.length=0]",
                response.ToString());
        }
        [TestMethod]
        public void TestBuild()
        {
            var response = NitricResponse
                .Build(HttpStatusCode.OK);

            Assert.AreEqual(200, (int)response.Status);

            response = NitricResponse
                .Build(HttpStatusCode.OK, "Hello World");

            Assert.AreEqual(200, response.Status);
            Assert.AreEqual("Hello World", Encoding.UTF8.GetString(response.Body));

            response = NitricResponse
                .Build("Hello World");

            Assert.AreEqual("Hello World", Encoding.UTF8.GetString(response.Body));
        }
    }
}
