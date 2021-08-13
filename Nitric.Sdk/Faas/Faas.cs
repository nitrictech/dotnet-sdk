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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using Util = Nitric.Api.Common.Util;
using Nitric.Proto.Faas.v1;
using ProtoClient = Nitric.Proto.Faas.v1.FaasService.FaasServiceClient;

namespace Nitric.Faas
{
    /**
     * <summary>Contains all the helper functions for Faas</summary>
     */
    public class Faas
    {
        private static readonly string DefaultHostName = "127.0.0.1";
        private static readonly int DefaultPort = 50051;

        public string Host { get; private set; }
        public ProtoClient Client { get; private set; }
        private INitricFunction Function;

        private Faas(INitricFunction function, string host, ProtoClient client)
        {
            this.Function = function;
            this.Host = host;
            this.Client = client;
        }

        public static void Start(INitricFunction function)
        {
            NewBuilder()
                .Function(function)
                .Build()
                .Start();
        }
        public void Start()
        {
            CallFunction().Wait();
        }
        private async Task CallFunction()
        {
            using (var call = this.Client.TriggerStream())
            {
                try
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                        switch (response.ContentCase)
                        {
                            case ServerMessage.ContentOneofCase.InitResponse:
                                break;
                            case ServerMessage.ContentOneofCase.TriggerRequest:
                                var trigger = Trigger.FromGrpcTriggerRequest(response.TriggerRequest);
                                //Call the function
                                var membraneMessage = Function.Handle(trigger);
                                var grpcMessage = membraneMessage.ToGrpcTriggerResponse();
                                //Write back the response to the server
                                await call.RequestStream.WriteAsync(
                                    new ClientMessage
                                    {
                                        Id = response.Id,
                                        TriggerResponse = grpcMessage
                                    }
                                );
                                break;
                            default:
                                //add error case
                                break;
                        }
                    }
                    await call.RequestStream.CompleteAsync();
                }
                catch (RpcException re)
                {
                    throw Api.Common.NitricException.FromRpcException(re);
                }
            }
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
                var client = new ProtoClient(channel);

                var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
                if (string.IsNullOrEmpty(childAddress))
                {
                    var hostn = hostName == null ? DefaultHostName : hostName;
                    var p = this.port == null ? DefaultPort : port;

                    childAddress = string.Format("{0}:{1}", hostn, p);
                }

                return new Faas(this.function, childAddress, client);
            }
        }
    }
}
