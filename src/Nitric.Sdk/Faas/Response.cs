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

using System.Collections.Generic;

using TriggerResponse = Nitric.Proto.Faas.v1.TriggerResponse;
using HttpResponseContextProto = Nitric.Proto.Faas.v1.HttpResponseContext;
using TopicResponseContextProto = Nitric.Proto.Faas.v1.TopicResponseContext;
using HeaderValue = Nitric.Proto.Faas.v1.HeaderValue;

namespace Nitric.Faas
{
    public class Response
    {
        public byte[] Data { get; set; }
        public ResponseContext Context { get; private set; }

        public Response(byte[] data, ResponseContext context)
        {
            this.Data = data;
            this.Context = context;
        }
        public TriggerResponse ToGrpcTriggerResponse()
        {
            if (this.Context.IsHttp())
            {
                var httpCtx = this.Context.AsHttp();
                var httpCtxBuilder = new HttpResponseContextProto {
                    Status = httpCtx.GetStatus(),
                };
                foreach (KeyValuePair<string, List<string>> kv in httpCtx.GetHeaders())
                {
                    var hv = new HeaderValue();
                    hv.Value.Add(kv.Value);
                    httpCtxBuilder.Headers.Add(kv.Key, hv);
                }

                return new TriggerResponse
                {
                    Data = Google.Protobuf.ByteString.CopyFrom(Data),
                    Http = httpCtxBuilder
                };
            } else if (this.Context.IsTopic())
            {
                var topicCtx = this.Context.AsTopic();
                var topicCtxBuilder = new TopicResponseContextProto {
                    Success = topicCtx.Success
                };

                return new TriggerResponse
                {
                    Data = Google.Protobuf.ByteString.CopyFrom(Data),
                    Topic = topicCtxBuilder
                };
            }
            return null;
        }
    }
}
