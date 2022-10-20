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
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
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
    public class ApiWorkerOptions : IFaasOptions
    {
        /// <summary>
        /// The name of the API this worker should register with.
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        /// The route this worker handlers.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// The HTTP method this worker can respond to.
        /// </summary>
        public HashSet<HttpMethod> Methods { get; set; }
    }

    /// <summary>
    /// Options for subscription trigger handling workers.
    /// </summary>
    public class SubscriptionWorkerOptions : IFaasOptions
    {
        /// <summary>
        ///
        /// </summary>
        public string Topic { get; set; }
    }

    /// <summary>
    /// Options for schedule trigger handling workers.
    /// </summary>
    public class ScheduleWorkerOptions : IFaasOptions
    {
        public string Description { get; set; }
        public int Rate { get; set; }
        public Frequency Frequency { get; set; }
    }

    /// <summary>
    /// Function as a Service server.
    ///
    /// Registers itself with a Nitric Server then routes incoming request to the appropriate workers.
    /// </summary>
    public class Faas
    {
        /// <summary>
        /// Function a as service options
        /// </summary>
        public IFaasOptions Options { get; set; }

        /// <summary>
        /// A handler for HTTP requests
        /// </summary>
        public Func<HttpContext, HttpContext> HttpHandler { get; set; }

        /// <summary>
        /// A handler for event requests
        /// </summary>
        public Func<EventContext, EventContext> EventHandler { get; set; }

        public ProtoClient Client { get; } = new ProtoClient(Util.GrpcChannelProvider.GetChannel());

        private static InitRequest OptionsToInit(IFaasOptions options)
        {
            switch (options)
            {
                case ApiWorkerOptions a:
                    var apiInitReq = new InitRequest { Api = new ApiWorker { Api = a.Api, Path = a.Route } };
                    apiInitReq.Api.Methods.Add(a.Methods.Select(m => m.ToString()));
                    return apiInitReq;
                case ScheduleWorkerOptions s:
                    var scheduleInitReq = new InitRequest
                    {
                        Schedule = new ScheduleWorker
                        {
                            Rate = new ScheduleRate { Rate = s.Rate + " " + s.Frequency.ToString().ToLower() },
                            Key = s.Description
                        }
                    };
                    return scheduleInitReq;
                case SubscriptionWorkerOptions s:
                    var subInitReq = new InitRequest
                    {
                        Subscription = new SubscriptionWorker
                        {
                            Topic = s.Topic,
                        },
                    };
                    return subInitReq;
            }

            throw new Exception("Invalid worker options");
        }

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

            await call.RequestStream.WriteAsync(
                new ClientMessage
                {
                    InitRequest = OptionsToInit(this.Options),
                }
            );

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
                                    resultContext = this.HttpHandler(ctxHttp);
                                    break;
                                case TriggerRequest.ContextOneofCase.Topic:
                                    if (this.EventHandler == null)
                                    {
                                        throw new UnimplementedException("Cannot handle event requests.");
                                    }

                                    var ctxEvent = TriggerContext<EventRequest, EventResponse>
                                        .FromGrpcTriggerRequest<EventContext>(grpcRequest.TriggerRequest);
                                    resultContext = this.EventHandler(ctxEvent);
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

                // await call.RequestStream.CompleteAsync();
            }
            catch (RpcException re)
            {
                throw Sdk.Common.NitricException.FromRpcException(re);
            }
        }
    }
}
