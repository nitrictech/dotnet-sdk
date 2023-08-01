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
using Nitric.Sdk.Resource;
using Nitric.Sdk.Storage;
using Util = Nitric.Sdk.Common.Util;
using ProtoApiWorkerOptions = Nitric.Proto.Faas.v1.ApiWorkerOptions;
using ProtoClient = Nitric.Proto.Faas.v1.FaasService.FaasServiceClient;
using BucketNotificationTypeProto = Nitric.Proto.Faas.v1.BucketNotificationType;

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

        
        public MethodOptions Options { get; set; }
    }

    public class MethodOptions
    {
        public Dictionary<string, string[]> Security { get; internal set; }
        public Dictionary<string, SecurityDefinition> SecurityDefs { get; internal set; }
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
    /// Options for schedule trigger handling workers by a rate and frequency descriptors.
    /// </summary>
    public class ScheduleRateWorkerOptions : IFaasOptions
    {
        public string Description { get; set; }
        public int Rate { get; set; }
        public Frequency Frequency { get; set; }
    }

    /// <summary>
    /// Options for scheduling triggers for workers by a cron expression.
    /// </summary>
    public class ScheduleCronWorkerOptions : IFaasOptions
    {
        public string Description { get; set; }
        public string Cron { get; set; }
    }

    public class FileNotificationWorkerOptions : IFaasOptions
    {
        public Bucket Bucket { get; set; }

        public BucketNotificationTypeProto NotificationType { get; set; }

        public string NotificationPrefixFilter { get; set; }

        public FileNotificationWorkerOptions(
            Bucket bucket,
            BucketNotificationType notificationType,
            string notificationPrefixFilter)
        {
            this.Bucket = bucket;
            this.NotificationType = ToGrpcNotificationType(notificationType);
            this.NotificationPrefixFilter = notificationPrefixFilter;

        }

        /// <summary>
        /// Converts a Nitric bucket notification type to the gRPC equivalent for over-the-wire transfer.
        /// </summary>
        /// <param name="notificationType">The type of notification that the bucket should be triggered on</param>
        /// <returns>A gRPC representation of the bucket notification type</returns>
        /// <exception cref="ArgumentException">If there is no matching notification type</exception>
        private static BucketNotificationTypeProto ToGrpcNotificationType(BucketNotificationType notificationType)
        {
            return notificationType switch
            {
                BucketNotificationType.Write => BucketNotificationTypeProto.Created,
                BucketNotificationType.Delete => BucketNotificationTypeProto.Deleted,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }
    }

    public class BucketNotificationWorkerOptions : IFaasOptions
    {
        public string Bucket { get; set; }
        public BucketNotificationTypeProto NotificationType { get; set; }

        public string NotificationPrefixFilter { get; set; }

        public BucketNotificationWorkerOptions(
            string bucket,
            BucketNotificationType notificationType,
            string notificationPrefixFilter)
        {
            this.Bucket = bucket;
            this.NotificationType = ToGrpcNotificationType(notificationType);
            this.NotificationPrefixFilter = notificationPrefixFilter;

        }

        /// <summary>
        /// Converts a Nitric bucket notification type to the gRPC equivalent for over-the-wire transfer.
        /// </summary>
        /// <param name="notificationType">The type of notification that the bucket should be triggered on</param>
        /// <returns>A gRPC representation of the bucket notification type</returns>
        /// <exception cref="ArgumentException">If there is no matching notification type</exception>
        private static BucketNotificationTypeProto ToGrpcNotificationType(BucketNotificationType notificationType)
        {
            return notificationType switch
            {
                BucketNotificationType.Write => BucketNotificationTypeProto.Created,
                BucketNotificationType.Delete => BucketNotificationTypeProto.Deleted,
                _ => throw new ArgumentException("Unsupported bucket notification type")
            };
        }
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
        public IFaasOptions Options { get; }

        /// <summary>
        /// Handlers for HTTP requests
        /// </summary>
        private Func<HttpContext, HttpContext> HttpHandler;

        private List<Middleware<HttpContext>> HttpHandlers = new List<Middleware<HttpContext>>();

        /// <summary>
        /// Handlers for event requests
        /// </summary>
        private Func<EventContext, EventContext> EventHandler;

        public List<Middleware<EventContext>> EventHandlers = new List<Middleware<EventContext>>();

        /// <summary>
        /// Handlers for bucket notification requests
        /// </summary>
        private Func<BucketNotificationContext, BucketNotificationContext> BucketNotificationHandler;

        private List<Middleware<BucketNotificationContext>> BucketNotificationHandlers =
            new List<Middleware<BucketNotificationContext>>();

        private Func<FileNotificationContext, FileNotificationContext> FileNotificationHandler;

        public List<Middleware<FileNotificationContext>> FileNotificationHandlers =
            new List<Middleware<FileNotificationContext>>();


        public ProtoClient Client { get; } = new ProtoClient(Util.GrpcChannelProvider.GetChannel());

        public Faas(IFaasOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Add a handler to the list of HTTP handlers. Used to chain together HTTP middleware.
        /// </summary>
        /// <param name="handler">The HTTP handler to add to the handlers list</param>
        /// <returns>A reference to this Faas object</returns>
        public Faas Http(Func<HttpContext, HttpContext> handler)
        {
            this.HttpHandler = handler;

            return this;
        }

        public Faas Http(Middleware<HttpContext>[] middleware)
        {
            this.HttpHandlers.AddRange(middleware);

            return this;
        }

        /// <summary>
        /// Add a handler to the list of event handlers. Used to chain together event middleware.
        /// </summary>
        /// <param name="handler">The event handler to add to the handlers list</param>
        /// <returns>A reference to this Faas object</returns>
        public Faas Event(Func<EventContext, EventContext> handler)
        {
            this.EventHandler = handler;

            return this;
        }

        public Faas Event(Middleware<EventContext>[] middleware)
        {
            this.EventHandlers.AddRange(middleware);

            return this;
        }

        /// <summary>
        /// Add a handler to the list of bucket notification handlers. Used to chain together notification middleware.
        /// </summary>
        /// <param name="handler">The bucket notification to add to the handlers list</param>
        /// <returns>A reference to this Faas object</returns>
        public Faas BucketNotification(Func<BucketNotificationContext, BucketNotificationContext> handler)
        {
            this.BucketNotificationHandler = handler;

            return this;
        }

        public Faas BucketNotification(Middleware<BucketNotificationContext>[] middleware)
        {
            this.BucketNotificationHandlers.AddRange(middleware);

            return this;
        }

        /// <summary>
        /// Add a handler to the list of bucket notification handlers. Used to chain together notification middleware.
        /// </summary>
        /// <param name="handler">The bucket notification to add to the handlers list</param>
        /// <returns>A reference to this Faas object</returns>
        public Faas FileNotification(Func<FileNotificationContext, FileNotificationContext> handler)
        {
            this.FileNotificationHandler = handler;

            return this;
        }

        public Faas FileNotification(Middleware<FileNotificationContext>[] middleware)
        {
            this.FileNotificationHandlers.AddRange(middleware);

            return this;
        }

        private static InitRequest OptionsToInit(IFaasOptions options)
        {
            switch (options)
            {
                case ApiWorkerOptions a:
                    var apiInitReq = new InitRequest { Api = new ApiWorker { Api = a.Api, Path = a.Route } };

                    apiInitReq.Api.Methods.Add(a.Methods.Select(m => m.ToString()));

                    var opts = new ProtoApiWorkerOptions();

                    if (a.Options.Security.Count == 0)
                    {
                        opts.SecurityDisabled = false;
                    } else
                    {
                        foreach (KeyValuePair<string, string[]> kv in a.Options.Security)
                        {
                            var scopes = new ApiWorkerScopes();
                            scopes.Scopes.AddRange(kv.Value);

                            opts.Security.Add(kv.Key, scopes);
                        }
                    }

                    apiInitReq.Api.Options = opts;

                    return apiInitReq;
                case ScheduleRateWorkerOptions s:
                    var scheduleInitReq = new InitRequest
                    {
                        Schedule = new ScheduleWorker
                        {
                            Rate = new ScheduleRate { Rate = s.Rate + " " + s.Frequency.ToString().ToLower() },
                            Key = s.Description
                        }
                    };
                    return scheduleInitReq;
                case ScheduleCronWorkerOptions s:
                    var scheduleCronInitReq = new InitRequest
                    {
                        Schedule = new ScheduleWorker
                        {
                            Cron =
                            {
                                Cron = s.Cron,
                            },
                            Key = s.Description,
                        }
                    };
                    return scheduleCronInitReq;
                case SubscriptionWorkerOptions s:
                    var subInitReq = new InitRequest
                    {
                        Subscription = new SubscriptionWorker
                        {
                            Topic = s.Topic,
                        },
                    };
                    return subInitReq;
                case BucketNotificationWorkerOptions b:
                    var notificationInitReq = new InitRequest
                    {
                        BucketNotification = new BucketNotificationWorker
                        {
                            Bucket = b.Bucket,
                            Config = new BucketNotificationConfig
                            {
                                NotificationPrefixFilter = b.NotificationPrefixFilter,
                                NotificationType = b.NotificationType,
                            }
                        }
                    };
                    return notificationInitReq;
                case FileNotificationWorkerOptions f:
                    var fNotificationInitRequest = new InitRequest
                    {
                        BucketNotification = new BucketNotificationWorker
                        {
                            Bucket = f.Bucket.Name,
                            Config = new BucketNotificationConfig
                            {
                                NotificationPrefixFilter = f.NotificationPrefixFilter,
                                NotificationType = f.NotificationType,
                            }
                        }
                    };
                    return fNotificationInitRequest;
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
            if (
                this.EventHandlers.Count == 0 && this.EventHandler == null &&
                this.HttpHandlers.Count == 0 && this.HttpHandler == null &&
                this.BucketNotificationHandlers.Count == 0 && this.BucketNotificationHandler == null &&
                this.FileNotificationHandlers.Count == 0 && this.FileNotificationHandler == null
                )
            {
                throw new Exception("At least one handler must be provided.");
            }

            using var call = this.Client.TriggerStream();

            try
            {
                await call.RequestStream.WriteAsync(
                    new ClientMessage
                    {
                        InitRequest = OptionsToInit(this.Options),
                    }
                );
            }
            catch (RpcException re)
            {
                // If the server is unavailable, provide a informative message
                throw re.StatusCode == StatusCode.Unavailable ?
                    new Exception(
                        "Unable to connect to a nitric server! If you're running locally make sure to run \"nitric start\"")
                    : re;
            }

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
                                    if (this.HttpHandlers.Count == 0 && this.HttpHandler == null)
                                    {
                                        throw new UnimplementedException("Cannot handle HTTP requests.");
                                    }

                                    var ctxHttp = TriggerContext<HttpRequest, HttpResponse>
                                        .FromGrpcTriggerRequest<HttpContext>(grpcRequest.TriggerRequest, this.Options);

                                    Func<HttpContext, HttpContext> composedHttpHandler = this.HttpHandler;
                                    if (this.HttpHandler == null)
                                    {
                                        Func<HttpContext, HttpContext> lastCall = (context) => context;

                                        this.HttpHandlers.Reverse();

                                        composedHttpHandler = this.HttpHandlers.Aggregate(lastCall, (next, handler) =>
                                        {
                                            Func<HttpContext, HttpContext> nextFunc = (context) => handler(context, next) ?? context;

                                            return nextFunc;
                                        });
                                    }

                                    resultContext = composedHttpHandler(ctxHttp);
                                    break;
                                case TriggerRequest.ContextOneofCase.Topic:
                                    if (this.EventHandlers.Count == 0 && this.EventHandler == null)
                                    {
                                        throw new UnimplementedException("Cannot handle event requests.");
                                    }

                                    var ctxEvent = TriggerContext<EventRequest, EventResponse>
                                        .FromGrpcTriggerRequest<EventContext>(grpcRequest.TriggerRequest, this.Options);


                                    Func<EventContext, EventContext> composedEventHandler = this.EventHandler;
                                    if (this.EventHandler == null)
                                    {
                                        Func<EventContext, EventContext> lastCall = (context) => context;

                                        this.EventHandlers.Reverse();

                                        composedEventHandler = this.EventHandlers.Aggregate(lastCall, (next, handler) =>
                                        {
                                            Func<EventContext, EventContext> nextFunc = (context) => handler(context, next) ?? context;

                                            return nextFunc;
                                        });

                                    }

                                    resultContext = composedEventHandler(ctxEvent);

                                    break;
                                case TriggerRequest.ContextOneofCase.Notification:
                                    if (this.Options.GetType() == typeof(BucketNotificationWorkerOptions))
                                    {
                                        if (this.BucketNotificationHandlers.Count == 0 &&
                                            this.BucketNotificationHandler == null)
                                        {
                                            throw new UnimplementedException(
                                                "Cannot handle bucket notification requests.");
                                        }

                                        var ctxNotification =
                                            TriggerContext<BucketNotificationRequest, BucketNotificationResponse>
                                                .FromGrpcTriggerRequest<BucketNotificationContext>(
                                                    grpcRequest.TriggerRequest, this.Options);


                                        var composedNotificationHandler = this.BucketNotificationHandler;
                                        if (this.BucketNotificationHandler == null)
                                        {
                                            Func<BucketNotificationContext, BucketNotificationContext> lastCall =
                                                (context) => context;

                                            this.BucketNotificationHandlers.Reverse();

                                            composedNotificationHandler = this.BucketNotificationHandlers.Aggregate(
                                                lastCall,
                                                (next, handler) =>
                                                {
                                                    Func<BucketNotificationContext, BucketNotificationContext>
                                                        nextFunc =
                                                            (context) => handler(context, next) ?? context;

                                                    return nextFunc;
                                                });

                                        }

                                        resultContext = composedNotificationHandler(ctxNotification);
                                    }
                                    else
                                    {
                                         if (this.FileNotificationHandlers.Count == 0 &&
                                            this.FileNotificationHandler == null)
                                        {
                                            throw new UnimplementedException(
                                                "Cannot handle file notification requests.");
                                        }

                                        var ctxNotification =
                                            TriggerContext<FileNotificationRequest, BucketNotificationResponse>
                                                .FromGrpcTriggerRequest<FileNotificationContext>(
                                                    grpcRequest.TriggerRequest, this.Options);


                                        var composedNotificationHandler = this.FileNotificationHandler;
                                        if (this.FileNotificationHandler == null)
                                        {
                                            Func<FileNotificationContext, FileNotificationContext> lastCall =
                                                (context) => context;

                                            this.FileNotificationHandlers.Reverse();

                                            composedNotificationHandler = this.FileNotificationHandlers.Aggregate(
                                                lastCall,
                                                (next, handler) =>
                                                {
                                                    Func<FileNotificationContext, FileNotificationContext>
                                                        nextFunc =
                                                            (context) => handler(context, next) ?? context;

                                                    return nextFunc;
                                                });

                                        }

                                        resultContext = composedNotificationHandler(ctxNotification);
                                    }

                                    break;
                                case TriggerRequest.ContextOneofCase.None:
                                default:
                                    throw new Exception("Unsupported trigger request type");
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
            }
            catch (RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}
