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

        public TopicTriggerContext AsTopic()
        {
            if (this.IsTopic())
            {
                return this as TopicTriggerContext;
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
