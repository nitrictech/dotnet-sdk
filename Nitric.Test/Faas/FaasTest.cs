using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using Nitric.Api.Faas;

namespace Nitric.Test.Api.Faas
{
    //[TestClass]
    public class FaasTest
    {
        const int TestingPort = 8081;
        [TestMethod]
        public void TestPort()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            Assert.AreEqual(faas.Port, 8080);
        }
        [TestMethod]
        public void TestHostName()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            Assert.AreEqual(faas.Hostname, "127.0.0.1");
        }
        [TestMethod]
        public void TestNewHostName()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            faas.Hostname = "localhost";
            Assert.AreEqual(faas.Hostname, "localhost");
        }
        [TestMethod]
        public void TestNewPort()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            faas.Port = TestingPort;
            Assert.AreEqual(TestingPort, faas.Port);
        }
        [TestMethod]
        public void TestStart()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            faas.Port = TestingPort;
            Thread faasThread1 = new Thread(() => faas.Start(new HelloWorld()));
            faasThread1.Start();
            Assert.IsNotNull(faas.Listener);

            //Resets the Faas listener
            faas.Listener.Close();
            faas.Listener = null;

            //Checks if it can start again
            Thread faasThread2 = new Thread(() => faas.Start(new HelloWorld()));
            faasThread2.Start();

            Assert.IsNotNull(faas.Listener);
            faas.Listener.Close();
        }
        [TestMethod]
        public void TestDoubleStart()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            Thread faasThread = new Thread(() => faas.Start(new HelloWorld()));
            faasThread.Start();
            Assert.ThrowsException<ArgumentException>(() => faas.Start(new HelloWorld()));
            faas.Listener.Close();
        }
        [TestMethod]
        public void TestCall()
        {
            Nitric.Api.Faas.Faas faas = new Nitric.Api.Faas.Faas();
            Thread faasThread = new Thread(() => faas.Start(new HelloWorld()));
            faasThread.Start();
            var client = new HttpClient();
            var result = client.GetAsync(string.Format("http://{0}:{1}/", "127.0.0.1", 8080));
            Assert.AreEqual("Hello World", result.Result);
        }
    }
    public class HelloWorld : INitricFunction
    {
        public int requestNum = 0;
        public NitricRequest[] Requests { get; private set; }
        public NitricResponse Handle(NitricRequest request)
        {
            Requests[requestNum++] = request;
            return new NitricResponse.Builder().Build("Hello World");
        }
    }
}
