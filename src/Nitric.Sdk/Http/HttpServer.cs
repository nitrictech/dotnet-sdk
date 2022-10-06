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
﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Nitric.Api.Http
{
    public class HttpServer
    {
        private static readonly string DefaultHostName = "127.0.0.1";
        private static readonly int DefaultPort = 8080;

        public bool Shutdown { get; set; }

        private string hostname = DefaultHostName;
        public string Hostname
        {
            get => hostname;
            set
            {
                hostname = value;
            }
        }
        private int port = DefaultPort;
        public int Port
        {
            get => port;
            set
            {
                port = value;
            }
        }
        public HttpListener Listener { get; set; }
        readonly Dictionary<string, IHttpHandler> pathFunctions = new Dictionary<string, IHttpHandler>();

        public HttpServer Register(string path, IHttpHandler function)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            if (pathFunctions.ContainsKey(path))
            {
                var msg = pathFunctions[path].GetType().Name + " already registered for path: " + path;
                throw new ArgumentException(msg);
            }
            pathFunctions.Add(path, function);
            return this;
        }
        public void Start(IHttpHandler function)
        {
            Register("/", function);
            Start();
        }
        public void Start()
        {
            if (Listener != null)
            {
                throw new ArgumentException("Listener has already started");
            }
            long time = DateTime.Now.Millisecond;

            var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
            if (!string.IsNullOrEmpty(childAddress))
            {
                Hostname = childAddress;
            }
            this.Listener = new HttpListener();
            foreach (string s in pathFunctions.Keys)
            {
                var function = pathFunctions[s];
                Listener.Prefixes.Add(string.Format("http://{0}:{1}/{2}/", Hostname, Port, s));
            }
            Listener.Start();

            //Prints out the 
            var builder = new StringBuilder().Append(GetType().Name);
            if (Hostname == DefaultHostName)
            {
                builder.Append(" listening on port ").Append(Port);
            }
            else
            {
                builder.Append(" listening on ").Append(Hostname).Append(":").Append(Port);
            }

            if (pathFunctions.Count == 0)
            {
                builder.Append(" - WARN No functions registered");
            }
            else if (pathFunctions.Count == 1)
            {
                builder.Append(" with function:");
            }
            else if (pathFunctions.Count > 1)
            {
                builder.Append(" with functions:");
            }
            Console.WriteLine(builder);


            if (pathFunctions.Count > 0)
            {
                foreach (string path in pathFunctions.Keys)
                {
                    var functionClass = pathFunctions[path].GetType();
                    var functionClassName = !string.IsNullOrEmpty(functionClass.Name)
                            ? functionClass.Name : functionClass.FullName;

                    Console.WriteLine("{0}\t-> {1}\n", path, functionClassName);
                }
            }
            //Loops until theres a connection is heard
            while (!Shutdown)
            {
                Task<HttpListenerContext> taskContext = Task.Run(async () => await Listener.GetContextAsync());
                OnContext(taskContext.Result);
            }
            Listener.Close();
            Shutdown = false;
        }
        private void OnContext(HttpListenerContext ctx)
        {
            if (pathFunctions.ContainsKey(ctx.Request.RawUrl))
            {
                var function = pathFunctions[ctx.Request.RawUrl];

                Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " Handling request");
                ctx.Response.ContentType = "text/plain";

                //Reads the request input stream
                var requestStreamReader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
                var requestBody = requestStreamReader.ReadToEnd();
                requestStreamReader.Close();

                //Builds a new NitricRequest based on the HttpContextRequest
                var requestBuilder = new HttpRequest.Builder()
                    .Path(ctx.Request.RawUrl)
                    .Query(ctx.Request.QueryString.ToString())
                    .Method(ctx.Request.HttpMethod)
                    .Body(Encoding.UTF8.GetBytes(requestBody.ToString().ToCharArray()));

                if (ctx.Request.Headers != null)
                {
                    var requestHeaders = new Dictionary<string, List<string>>();
                    foreach (KeyValuePair<string, List<string>> header in ctx.Request.Headers)
                    {
                        var key = header.Key;
                        if (key != "Connection" && key != "Host" && key != "Content-length")
                        {
                            var headerList = requestHeaders[key];
                            if (headerList == null)
                            {
                                headerList = new List<string>(1);
                                requestHeaders.Add(key, headerList);
                            }
                            headerList.AddRange(header.Value);
                        }
                    }
                    requestBuilder.Headers(requestHeaders);
                }

                var request = requestBuilder.Build();

                var response = function.Handle(request);

                //Converts the NitricResponse object into a HttpResponse 
                foreach (KeyValuePair<string, List<string>> entry in response.Headers)
                {
                    ctx.Response.AddHeader(entry.Key, entry.Value.ToString());
                }
                ctx.Response.StatusCode = (int)response.Status;
                ctx.Response.OutputStream.Write(response.Body, 0, response.Body.Length);
                ctx.Response.Close();

                Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " completed");
            }
        }
    }
}
