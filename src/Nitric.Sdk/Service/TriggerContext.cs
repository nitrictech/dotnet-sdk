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

namespace Nitric.Sdk.Service
{
    /// <summary>
    /// The base request structure, common to HTTP and Event requests.
    /// </summary>
    public abstract class TriggerRequest { }

    public abstract class TriggerResponse { }

    /// <summary>
    /// The base context structure, common to HTTP and Event contexts.
    /// </summary>
    /// <typeparam name="Request">The context's request.</typeparam>
    /// <typeparam name="Response">The context's response.</typeparam>
    public abstract class TriggerContext<Request, Response>
        where Request : TriggerRequest
        where Response : TriggerResponse
    {
        protected string Id;
        public Request Req;
        public Response Res;

        /// <summary>
        /// Create a new trigger context with the provided request and response objects.
        /// </summary>
        /// <param name="req">The request object that initiated the trigger</param>
        /// <param name="res">The response to be returned from processing the trigger</param>
        protected TriggerContext(string id, Request req, Response res)
        {
            this.Id = id;
            this.Req = req;
            this.Res = res;
        }
    }
}
