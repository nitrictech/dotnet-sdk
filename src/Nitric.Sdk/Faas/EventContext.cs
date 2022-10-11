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

using Nitric.Faas;
using Nitric.Sdk.Common;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Sdk.Faas
{
    public class EventRequest : AbstractRequest
    {
        public string Topic { get; private set; }

        public EventRequest(byte[] data, string topic) : base(data)
        {
            this.Topic = topic;
        }
    }

    public class EventResponse
    {
        public bool Success { get; set; }

        public EventResponse(bool success)
        {
            this.Success = success;
        }
    }

    public class EventContext : TriggerContext<EventRequest, EventResponse>
    {
        public string Topic { get; private set; }
        public EventContext(EventRequest req, EventResponse res, string topic): base(req, res)
        {
            this.Topic = topic;
        }

        public static EventContext FromGrpcTriggerRequest(TriggerRequestProto trigger)
        {
            // TODO: implement
            throw new UnimplementedException("unimplemented");
        }
    }
}
