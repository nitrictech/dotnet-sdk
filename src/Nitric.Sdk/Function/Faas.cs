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
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Nitric.Proto.Faas.v1;
using Nitric.Sdk.Common;
using Util = Nitric.Sdk.Common.Util;
using ProtoClient = Nitric.Proto.Faas.v1.FaasService.FaasServiceClient;

namespace Nitric.Sdk.Function
{
    /// <summary>
    /// Supported HTTP request methods
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// GET
        /// </summary>
        GET,
        /// <summary>
        /// POST
        /// </summary>
        POST,
        /// <summary>
        /// PUT
        /// </summary>
        PUT,
        /// <summary>
        /// DELETE
        /// </summary>
        DELETE,
        /// <summary>
        /// PATCH
        /// </summary>
        PATCH,
        /// <summary>
        /// OPTIONS
        /// </summary>
        OPTIONS,
    }

    /// <summary>
    /// Common Function as a Service worker options
    /// </summary>
    public interface IFaasOptions
    {
    }

    /// <summary>
    /// Options for API request handling workers.
    /// </summary>
    public class ApiWorkerOptions: IFaasOptions
    {
        private string api;
        private string route;
        private HashSet<HttpMethod> method;

        /// <summary>
        /// Construct a new API worker options object.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="route"></param>
        /// <param name="method"></param>
        public ApiWorkerOptions(string api, string route, HashSet<HttpMethod> method)
        {
            this.api = api;
            this.route = route;
            this.method = method;
        }
    }

    /// <summary>
    /// Options for subscription trigger handling workers.
    /// </summary>
    public class SubscriptionWorkerOptions: IFaasOptions
    {
        private string topic;

        /// <summary>
        /// Construct a new subscription worker options object.
        /// </summary>
        /// <param name="topic"></param>
        public SubscriptionWorkerOptions(string topic)
        {
            this.topic = topic;
        }
    }

    /// <summary>
    /// Options for schedule trigger handling workers.
    /// </summary>
    public class ScheduleWorkerOptions : IFaasOptions
    {
        private string description;
        private int rate;
        private Frequency frequency;

        /// <summary>
        /// Construct a options for a schedule worker.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="rate"></param>
        /// <param name="frequency"></param>
        public ScheduleWorkerOptions(string description, int rate, Frequency frequency)
        {
            this.description = description;
            this.rate = rate;
            this.frequency = frequency;
        }
    }

    /// <summary>
    /// Function as a Service server.
    ///
    /// Registers itself with a Nitric Server then routes incoming request to the appropriate workers.
    /// </summary>
    public class Faas
    {
        private IFaasOptions Options { get; set; }
        private IHandler<HttpContext> HttpHandler { get; set; }
        private IHandler<EventContext> EventHandler { get; set; }
        private ProtoClient Client { get; } = new ProtoClient(Util.GrpcChannelProvider.GetChannel());

        /// <summary>
        /// Start the FaaS service to start receiving requests from the Nitric Server
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnimplementedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NitricException"></exception>
        public async Task Start()
        {
            if (this.EventHandler == null && this.HttpHandler == null)
            {
                throw new Exception("At least one handler must be provided.");
            }

            using var call = this.Client.TriggerStream();
            try
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var grpcRequest = call.ResponseStream.Current;
                    switch (grpcRequest.ContentCase)
                    {
                        case ServerMessage.ContentOneofCase.InitResponse:
                            break; //no-op - reserved for future use.
                        case ServerMessage.ContentOneofCase.TriggerRequest:
                            IContext resultContext;
                            switch (grpcRequest.TriggerRequest.ContextCase)
                            {
                                case TriggerRequest.ContextOneofCase.Http:
                                    if (this.HttpHandler == null)
                                    {
                                        throw new UnimplementedException("Cannot handle HTTP requests.");
                                    }

                                    var ctxHttp = TriggerContext<HttpRequest, HttpResponse>
                                        .FromGrpcTriggerRequest<HttpContext>(grpcRequest.TriggerRequest);
                                    resultContext = this.HttpHandler.Invoke(ctxHttp);
                                    break;
                                case TriggerRequest.ContextOneofCase.Topic:
                                    if (this.EventHandler == null)
                                    {
                                        throw new UnimplementedException("Cannot handle event requests.");
                                    }

                                    var ctxEvent = TriggerContext<EventRequest, EventResponse>
                                        .FromGrpcTriggerRequest<EventContext>(grpcRequest.TriggerRequest);
                                    resultContext = this.EventHandler.Invoke(ctxEvent);
                                    break;
                                case TriggerRequest.ContextOneofCase.None:
                                default:
                                    throw new Exception("Unsupported trigger request type");
                                    break;
                            }

                            var grpcResponse = resultContext.ToGrpcTriggerContext();

                            //Write back the response to the server
                            await call.RequestStream.WriteAsync(
                                new ClientMessage
                                {
                                    Id = grpcRequest.Id,
                                    TriggerResponse = grpcResponse
                                }
                            );
                            break;
                        case ServerMessage.ContentOneofCase.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                await call.RequestStream.CompleteAsync();
            }
            catch (RpcException re)
            {
                throw Sdk.Common.NitricException.FromRpcException(re);
            }
        }
    }
}
