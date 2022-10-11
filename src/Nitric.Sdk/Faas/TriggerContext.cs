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
using Nitric.Sdk.Common;
using Nitric.Sdk.Faas;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;

using HeaderValue = Nitric.Proto.Faas.v1.HeaderValue;
using QueryValue = Nitric.Proto.Faas.v1.QueryValue;

namespace Nitric.Faas
{
    public abstract class AbstractRequest
    {
        protected byte[] data;

        protected AbstractRequest(byte[] data)
        {
            this.data = data;
        }

        public string ToText()
        {
            return System.Text.Encoding.UTF8.GetString(this.data, 0, this.data.Length);
        }
    }
    public abstract class TriggerContext<Req, Res> where Req : AbstractRequest
    {
        protected Req req;
        protected Res res;

        /// <summary>
        /// Create a new trigger context with the provided request and response objects.
        /// </summary>
        /// <param name="req">The request object that initiated the trigger</param>
        /// <param name="res">The response to be returned from processing the trigger</param>
        protected TriggerContext(Req req, Res res)
        {
            this.req = req;
            this.res = res;
        }

        public static T FromGrpcTriggerRequest<T>(TriggerRequestProto trigger) where T : TriggerContext<Req, Res>
        {
            return trigger.ContextCase switch
            {
                TriggerRequestProto.ContextOneofCase.Http => HttpContext.FromGrpcTriggerRequest(trigger) as T,
                TriggerRequestProto.ContextOneofCase.Topic => EventContext.FromGrpcTriggerRequest(trigger) as T,
                _ => throw new Exception("Unsupported trigger request type")
            };
        }
    }
}
