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
using Newtonsoft.Json;
using Nitric.Proto.Faas.v1;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Sdk.Function
{
    /// <summary>
    /// The base request structure, common to HTTP and Event requests.
    /// </summary>
    public abstract class AbstractRequest
    {
        /// <summary>
        /// The payload of the request.
        /// </summary>
        protected byte[] data;

        /// <summary>
        /// Construct a request.
        /// </summary>
        /// <param name="data">The payload of the request.</param>
        protected AbstractRequest(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Convert the payload of the request to a string, assuming UTF-8 encoding.
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            return System.Text.Encoding.UTF8.GetString(this.data, 0, this.data.Length);
        }

        /// <summary>
        /// Get the HTTP body data encoded from json.
        ///
        /// The object is converted from a JSON string to a type T.
        /// </summary>
        public T FromJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.data.ToString());
        }

    }

    public interface IContext
    {
        /// <summary>
        /// Convert the context object to the gRPC wire representation.
        /// </summary>
        /// <returns>A TriggerResponse from the context</returns>
        public TriggerResponse ToGrpcTriggerContext();
    }

    /// <summary>
    /// The base context structure, common to HTTP and Event contexts.
    /// </summary>
    /// <typeparam name="Request">The context's request.</typeparam>
    /// <typeparam name="Response">The context's response.</typeparam>
    public abstract class TriggerContext<Request, Response> : IContext where Request : AbstractRequest
    {
        public Request Req;
        public Response Res;

        public abstract TriggerResponse ToGrpcTriggerContext();

        /// <summary>
        /// Create a new trigger context with the provided request and response objects.
        /// </summary>
        /// <param name="req">The request object that initiated the trigger</param>
        /// <param name="res">The response to be returned from processing the trigger</param>
        protected TriggerContext(Request req, Response res)
        {
            this.Req = req;
            this.Res = res;
        }

        /// <summary>
        /// Construct the appropriate context object based on the type of the incoming trigger.
        /// </summary>
        /// <param name="trigger">The trigger to use to create the context.</param>
        /// <typeparam name="T">The expected context type.</typeparam>
        /// <returns>A new context object.</returns>
        /// <exception cref="Exception">Throws if the context type is unknown or unsupported.</exception>
        public static T FromGrpcTriggerRequest<T>(TriggerRequestProto trigger, IFaasOptions options) where T : TriggerContext<Request, Response>
        {
            return trigger.ContextCase switch
            {
                TriggerRequestProto.ContextOneofCase.Http => HttpContext.FromGrpcTriggerRequest(trigger) as T,
                TriggerRequestProto.ContextOneofCase.Topic => EventContext.FromGrpcTriggerRequest(trigger) as T,
                TriggerRequestProto.ContextOneofCase.Notification =>
                    BucketNotificationContext.FromGrpcTriggerRequest(trigger, (BucketNotificationWorkerOptions)options) as T,
                _ => throw new Exception("Unsupported trigger request type")
            };
        }
    }
}
