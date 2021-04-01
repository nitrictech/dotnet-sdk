using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using Nitric.Api.Faas;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Nitric.Test
{
    //[TestClass]
    public class FaasTest
    {
        const int TestingPort = 8081;
        [TestMethod]
        public void TestPort()
        {
            Faas faas = new Faas();
            Assert.AreEqual(faas.Port, 8080);
        }
        [TestMethod]
        public void TestHostName()
        {
            Faas faas = new Faas();
            Assert.AreEqual(faas.HostName, "127.0.0.1");
        }
        [TestMethod]
        public void TestNewHostName()
        {
            Faas faas = new Faas();
            faas.HostName = "localhost";
            Assert.AreEqual(faas.HostName, "localhost");
        }
        [TestMethod]
        public void TestNewPort()
        {
            Faas faas = new Faas();
            faas.Port = TestingPort;
            Assert.AreEqual(TestingPort, faas.Port);
        }
        [TestMethod]
        public void TestStart()
        {
            Faas faas = new Faas();
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
            Faas faas = new Faas();
            Thread faasThread = new Thread(() => faas.Start(new HelloWorld()));
            faasThread.Start();
            Assert.ThrowsException<ArgumentException>(() => faas.Start(new HelloWorld()));
            faas.Listener.Close();
        }
        [TestMethod]
        public void TestCall()
        {
            Faas faas = new Faas();
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
