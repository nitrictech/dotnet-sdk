using System.Collections.Generic;
using Nitric.Api.Common;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Faas
{
    public abstract class TriggerContext
    {
        public bool IsHttp()
        {
            return this.GetType() == typeof(HttpRequestTriggerContext);
        }
        public bool IsTopic()
        {
            return this.GetType() == typeof(TopicTriggerContext);
        }
        public HttpRequestTriggerContext AsHttp()
        {
            if (this.IsHttp())
            {
                return this as HttpRequestTriggerContext;
            }
            return null;
        }
        public static TriggerContext FromGrpcTriggerRequest(TriggerRequestProto trigger)
        {
            switch (trigger.ContextCase){
                case TriggerRequestProto.ContextOneofCase.Http:
                    return new HttpRequestTriggerContext(
                        trigger.Http.Method,
                        trigger.Http.Path,
                        Util.CollectionToDict(trigger.Http.Headers),
                        Util.CollectionToDict(trigger.Http.QueryParams)
                    );
                case TriggerRequestProto.ContextOneofCase.Topic:
                    return new TopicTriggerContext(
                        trigger.Topic.Topic
                    );
            }
            return null;
        }
    }
}
