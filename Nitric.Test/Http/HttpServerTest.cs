using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using Nitric.Api.Http;

namespace Nitric.Test.Api.Http
{
    //[TestClass]
    public class HttpServerTest
    {
        const int TestingPort = 8081;
        [TestMethod]
        public void TestPort()
        {
            HttpServer httpServer = new HttpServer();
            Assert.AreEqual(httpServer.Port, 8080);
        }
        [TestMethod]
        public void TestHostName()
        {
            HttpServer httpServer = new HttpServer();
            Assert.AreEqual(httpServer.Hostname, "127.0.0.1");
        }
        [TestMethod]
        public void TestNewHostName()
        {
            HttpServer httpServer = new HttpServer();
            httpServer.Hostname = "localhost";
            Assert.AreEqual(httpServer.Hostname, "localhost");
        }
        [TestMethod]
        public void TestNewPort()
        {
            HttpServer httpServer = new HttpServer();
            httpServer.Port = TestingPort;
            Assert.AreEqual(TestingPort, httpServer.Port);
        }
        /*[TestMethod]
        public void TestRegister()
        {
            HttpServer httpServer = new HttpServer();

            try
            {
                httpServer.Register(null, new HelloWorld());
                Assert.IsTrue(false);
            } catch (ArgumentExceptionNull)
        }
        [TestMethod]
        public void TestStart()
        {
        }
        [TestMethod]
        public void TestDoubleStart()
        {
        }
        [TestMethod]
        public void TestCall()
        {
        }*/
    }
    public class HelloWorld : IHttpHandler
    {
        public int requestNum = 0;
        public HttpResponse Handle(HttpRequest request)
        {
            return new HttpResponse.Builder().Build("Hello World");
        }
    }
}
