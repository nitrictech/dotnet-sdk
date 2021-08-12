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
using Xunit;
using System;
using System.Net.Http;
using System.Threading;
using Nitric.Api.Http;
using System.Threading.Tasks;

namespace Nitric.Test.Api.Http
{
    //public class HttpServerTest
    //{
    //    const int TestingPort = 8081;
    //    [Fact]
    //    public void TestPort()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        Assert.Equal(8080, httpServer.Port);
    //    }
    //    [Fact]
    //    public void TestHostName()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        Assert.Equal("127.0.0.1", httpServer.Hostname);
    //    }
    //    [Fact]
    //    public void TestNewHostName()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        httpServer.Hostname = "localhost";
    //        Assert.Equal("localhost", httpServer.Hostname);
    //    }
    //    [Fact]
    //    public void TestNewPort()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        httpServer.Port = TestingPort;
    //        Assert.Equal(TestingPort, httpServer.Port);
    //    }
    //    [Fact]
    //    public void TestRegister()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        try
    //        {
    //            httpServer.Register(null, new HelloWorld());
    //            Assert.True(false);
    //        }
    //        catch (ArgumentNullException ane)
    //        {
    //            Assert.Equal("Value cannot be null. (Parameter 'path')", ane.Message);
    //        }
    //        try
    //        {
    //            httpServer.Register("path", null);
    //            Assert.True(false);
    //        }
    //        catch (ArgumentNullException ane)
    //        {
    //            Assert.Equal("Value cannot be null. (Parameter 'function')", ane.Message);
    //        }
    //        httpServer.Register("path", new HelloWorld());
    //        Assert.True(true); //The above shouldn't throw an error
    //    }
    //    //[Fact]
    //    public void TestStart()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        httpServer.Port = TestingPort;

    //        Thread httpServerThread = new Thread(() => httpServer.Start(new HelloWorld()));
    //        httpServerThread.Start();
    //        Assert.NotNull(httpServer.Listener);
    //        httpServer.Shutdown = true;

    //        //Resets the Http Listener listener
    //        httpServer.Listener = null;

    //        httpServerThread.Start();
    //        Assert.NotNull(httpServer.Listener);
    //        httpServer.Shutdown = true;
    //    }
    //    [Fact]
    //    public void TestDoubleStart()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        Thread httpServerThread = new Thread(() => httpServer.Start(new HelloWorld()));
    //        httpServerThread.Start();
    //        Assert.Throws<ArgumentException>(() => httpServer.Start(new HelloWorld()));
    //        httpServer.Listener.Close();
    //    }
    //    //[Fact]
    //    public void TestCall()
    //    {
    //        HttpServer httpServer = new HttpServer();
    //        Thread httpServerThread = new Thread(() => httpServer.Start(new HelloWorld()));
    //        httpServerThread.Start();
    //        var client = new HttpClient();
    //        var result = client.GetAsync(string.Format("http://{0}:{1}/", "127.0.0.1", 8080));
    //        Assert.Equal("Hello World", result.Result.ToString());
    //    }
    //}
    //public class HelloWorld : IHttpHandler
    //{
    //    public int requestNum = 0;
    //    public HttpResponse Handle(HttpRequest request)
    //    {
    //        return new HttpResponse.Builder().Build("Hello World");
    //    }
    //}
}
