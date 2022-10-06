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

using TriggerRequest = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Faas
{
    public class Trigger
    {
        public byte[] Data { get; private set; }
        public string MimeType { get; private set; }
        public TriggerContext Context { get; private set; }

        private Trigger(byte[] data, string mimeType, TriggerContext context)
        {
            this.Data = data;
            this.MimeType = mimeType;
            this.Context = context;
        }
        public static Trigger FromGrpcTriggerRequest(TriggerRequest trigger)
        {
            TriggerContext ctx = TriggerContext.FromGrpcTriggerRequest(trigger);

            return new Trigger(
                trigger.Data.ToByteArray(),
                trigger.MimeType,
                ctx
            );
        }
        public Response DefaultResponse()
        {
            return this.DefaultResponse(null);
        }
        public Response DefaultResponse(byte[] data)
        {
            ResponseContext responseCtx = null;

            if (this.Context.IsHttp())
            {
                responseCtx = new HttpResponseContext();
            } else if (this.Context.IsTopic())
            {
                responseCtx = new TopicResponseContext();
            }

            return new Response(data, responseCtx);
        }
    }
}
