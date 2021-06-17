using System.Collections.Generic;

using TriggerResponse = Nitric.Proto.Faas.v1.TriggerResponse;
using HttpResponseContextProto = Nitric.Proto.Faas.v1.HttpResponseContext;
using TopicResponseContextProto = Nitric.Proto.Faas.v1.TopicResponseContext;

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
                foreach (KeyValuePair<string,string> kv in httpCtx.GetHeaders())
                {
                    httpCtxBuilder.Headers.Add(kv.Key, kv.Value);
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
