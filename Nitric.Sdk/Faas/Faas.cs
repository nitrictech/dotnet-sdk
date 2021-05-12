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
using System;
using System.Net;
using System.IO;
using Nitric.Api.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
namespace Nitric.Faas
{
    /**
     * <summary>Contains all the helper functions for Faas</summary>
     */
    public class Faas
    {
        private static readonly string DefaultHostName = "127.0.0.1";
        private static readonly int DefaultPort = 8080;

        public string Host { get; private set; }

        private Faas(INitricFunction function, string host)
        {
            this.function = function;
            this.Host = host;
        }

        public HttpListener Listener { get; set; }
        INitricFunction function;

        public static void Start(INitricFunction function)
        {
            Faas
                .NewBuilder()
                .Function(function)
                .Build()
                .Start();
        }

        public void Start()
        {
            if (Listener != null)
            {
                throw new ArgumentException("listener has already started");
            }
            long time = DateTime.Now.Millisecond;

            Console.WriteLine(string.Format("Faas listening on {0} with function: {1}", this.Host, function.GetType().Name));
            this.Listener = new HttpListener();
            Listener.Prefixes.Add(string.Format("http://{0}/", this.Host));
            Listener.Start();
            while (true)
            {
                // Will wait here until we hear from a connection
                Task<HttpListenerContext> taskContext = Task.Run(async () => await Listener.GetContextAsync());
                OnContext(taskContext.Result);
            }
        }
        private void OnContext(HttpListenerContext ctx)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " Handling request");
            ctx.Response.ContentType = "text/plain";

            //Reads the request input stream
            var requestStreamReader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
            var requestBody = requestStreamReader.ReadToEnd();
            requestStreamReader.Close();
            //Builds a new NitricRequest based on the HttpContextRequest
            NitricRequest request = new NitricRequest.Builder()
                .Path(ctx.Request.RawUrl)
                .Headers(Util.NameValueCollecToDict(ctx.Request.Headers))
                .Query(ctx.Request.QueryString.ToString())
                .Method(ctx.Request.HttpMethod)
                .Body(Encoding.UTF8.GetBytes(requestBody.ToString().ToCharArray()))
                .Build();
            //Calls the user's NitricFunction handler, parsing in the request and returns the response
            var functionResponse = function.Handle(request);
            //Converts the NitricResponse object into a HttpResponse 
            foreach (KeyValuePair<string, List<string>> entry in functionResponse.Headers)
            {
                ctx.Response.AddHeader(entry.Key, entry.Value.ToString());
            }
            ctx.Response.StatusCode = (int)functionResponse.Status;
            ctx.Response.OutputStream.Write(functionResponse.Body, 0, functionResponse.Body.Length);
            ctx.Response.Close();
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " completed");
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private INitricFunction function;
            private int? port;
            private String hostName;

            public Builder Function(INitricFunction function)
            {
                this.function = function;
                return this;
            }

            public Builder Port(Int16 port)
            {
                this.port = port;
                return this;
            }

            public Builder HostName(string hostName)
            {
                this.hostName = hostName;
                return this;
            }

            public Builder() { }

            public Faas Build()
            {
                if (function == null)
                {
                    throw new ArgumentNullException("function");
                }

                var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
                if (!string.IsNullOrEmpty(childAddress))
                {
                    var hostn = hostName == null ? DefaultHostName : hostName;
                    var p = this.port == null ? DefaultPort : port;

                    childAddress = string.Format("{0}:{1}", hostn, p);
                }

                return new Faas(function, childAddress);
            }
        }
    }
}
