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

using System.Text;
using Newtonsoft.Json;
using Nitric.Proto.Faas.v1;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Sdk.Function
{
    /// <summary>
    /// Represents a message pushed to a subscriber via a topic.
    /// </summary>
    public class EventRequest : AbstractRequest
    {
        /// <summary>
        /// The name of the topic that triggered this request
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// Construct an event request
        /// </summary>
        /// <param name="data">the payload of the message</param>
        /// <param name="topic">the source topic</param>
        public EventRequest(byte[] data, string topic) : base(data)
        {
            this.Topic = topic;
        }

        public Event.Event<T> Payload<T>()
        {
            return Event.Event<T>.FromPayload(this.data);
        }
    }

    /// <summary>
    /// Represents the results of processing an event.
    /// </summary>
    public class EventResponse
    {
        /// <summary>
        /// Indicates whether the event was successfully processed.
        ///
        /// If this value is false, the event may be resent.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Construct an event response.
        /// </summary>
        /// <param name="success">Indicates whether the event was successfully processed.</param>
        public EventResponse(bool success)
        {
            this.Success = success;
        }
    }

    /// <summary>
    /// Represents the request/response context for an event.
    /// </summary>
    public class EventContext : TriggerContext<EventRequest, EventResponse>
    {
        /// <summary>
        /// Construct a new EventContext.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public EventContext(EventRequest req, EventResponse res) : base(req, res)
        {
        }

        /// <summary>
        /// Construct an event topic from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into an EventContext.</param>
        /// <returns>the new event context</returns>
        public static EventContext FromGrpcTriggerRequest(TriggerRequestProto trigger)
        {
            return new EventContext(new EventRequest(trigger.Data.ToByteArray(), trigger.Topic.Topic),
                new EventResponse(true));
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        public override TriggerResponse ToGrpcTriggerContext()
        {
            return new TriggerResponse { Topic = new TopicResponseContext { Success = this.Res.Success } };
        }
    }
}
