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
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using Util = Nitric.Api.Common.Util;
using Nitric.Proto.Faas.v1;
using GrpcClient = Nitric.Proto.Faas.v1.Faas.FaasClient;

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
        public GrpcClient Client { get; private set; }
        private INitricFunction Function;

        private Faas(INitricFunction function, string host, GrpcClient client)
        {
            this.function = function;
            this.Host = host;
        }

        public HttpListener Listener { get; set; }
        INitricFunction function;

        public static void Start(INitricFunction function)
        {
            NewBuilder()
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

            JsonParser formatter = new JsonParser(new JsonParser.Settings(100));

            // TODO: Add error case if we fail to do this
            var triggerRequest = formatter.Parse<TriggerRequest>(requestBody);

            //Add json to properties of trigger request
            var trigger = Trigger.FromGrpcTriggerRequest(triggerRequest);
            Response response = null;

            try
            {
                response = function.Handle(trigger);
            }
            catch (Exception e)
            {
                response = trigger.DefaultResponse(
                    Encoding.UTF8.GetBytes("An error occured, please see logs for details.\n")
                );
                if (response.Context.IsHttp())
                {
                    response.Context.AsHttp().SetStatus(500);
                    response.Context.AsHttp().AddHeader("Content-Type", "text/plain");
                }
            }
            var triggerResponse = response.ToGrpcTriggerResponse();
            var jsonResponse =
                new JsonFormatter(
                    new JsonFormatter.Settings(false)
                ).Format(triggerResponse);

            ctx.Response.Headers.Add("Content-Type", "application/json");
            ctx.Response.OutputStream.Write(Encoding.UTF8.GetBytes(jsonResponse), 0, jsonResponse.Length);

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
                var address = Util.GetEnvVar("SERVICE_ADDRESS");
                if (string.IsNullOrEmpty(address))
                {
                    address = string.Format("http://{0}:{1}", DefaultHostName, DefaultPort);
                }
                var channel = GrpcChannel.ForAddress(address);
                var client = new GrpcClient(channel);

                var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
                if (string.IsNullOrEmpty(childAddress))
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
