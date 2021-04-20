using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using Nitric.Api.Http;
using System.Threading.Tasks;

namespace Nitric.Test.Api.Http
{
    [TestClass]
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
        [TestMethod]
        public void TestRegister()
        {
            HttpServer httpServer = new HttpServer();
            try
            {
                httpServer.Register(null, new HelloWorld());
                Assert.IsTrue(false);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'path')", ane.Message);
            }
            try
            {
                httpServer.Register("path", null);
                Assert.IsTrue(false);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'function')", ane.Message);
            }
            httpServer.Register("path", new HelloWorld());
            Assert.IsTrue(true); //The above shouldn't throw an error
        }
        [TestMethod]
        public void TestStart()
        {
            HttpServer httpServer = new HttpServer();
            httpServer.Port = TestingPort;

            Thread httpServerThread = new Thread(() => httpServer.Start(new HelloWorld()));
            httpServerThread.Start();
            Assert.IsNotNull(httpServer.Listener);
            httpServer.Shutdown = true;

            //Resets the Http Listener listener
            httpServer.Listener = null;

            httpServerThread.Start();
            Assert.IsNotNull(httpServer.Listener);
            httpServer.Shutdown = true;
        }
        [TestMethod]
        public void TestDoubleStart()
        {
            HttpServer httpServer = new HttpServer();
            Thread httpServerThread = new Thread(() => httpServer.Start(new HelloWorld()));
            httpServerThread.Start();
            Assert.ThrowsException<ArgumentException>(() => httpServer.Start(new HelloWorld()));
            httpServer.Listener.Close();
        }
        [TestMethod]
        public void TestCall()
        {
            HttpServer httpServer = new HttpServer();
            Thread httpServerThread = new Thread(() => httpServer.Start(new HelloWorld()));
            httpServerThread.Start();
            var client = new HttpClient();
            var result = client.GetAsync(string.Format("http://{0}:{1}/", "127.0.0.1", 8080));
            Assert.AreEqual("Hello World", result.Result);
        }
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
