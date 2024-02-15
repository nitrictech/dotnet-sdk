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

using Nitric.Sdk.Common;
using Nitric.Proto.Topics.v1;
using ProtoMessageResponse = Nitric.Proto.Topics.v1.MessageResponse;
using System.Collections.Generic;

namespace Nitric.Sdk.Service
{
    /// <summary>
    /// Represents a message pushed to a subscriber via a topic.
    /// </summary>
    public class MessageRequest : TriggerRequest
    {
        /// <summary>
        /// The name of the topic that triggered this request
        /// </summary>
        public string TopicName { get; private set; }

        public Dictionary<string, object> Payload { get; private set; }

        /// <summary>
        /// Construct an event request
        /// </summary>
        /// <param name="topicName">the source topic</param>
        /// <param name="message">the message payload</param>
        public MessageRequest(string topicName, Message message) : base()
        {
            this.TopicName = topicName;
            this.Payload = Struct.ToDictionary(message.StructPayload);
        }
    }

    /// <summary>
    /// Represents the results of processing an event.
    /// </summary>
    public class MessageResponse : TriggerResponse
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
        public MessageResponse(bool success)
        {
            this.Success = success;
        }
    }

    /// <summary>
    /// Represents the request/response context for an event.
    /// </summary>
    public class MessageContext : TriggerContext<MessageRequest, MessageResponse>
    {
        /// <summary>
        /// Construct a new EventContext.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public MessageContext(string id, MessageRequest req, MessageResponse res) : base(id, req, res)
        {
        }

        /// <summary>
        /// Construct an event topic from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into an EventContext.</param>
        /// <returns>the new event context</returns>
        protected static MessageContext FromRequest(ServerMessage trigger)
        {
            return new MessageContext(trigger.Id, new MessageRequest(trigger.MessageRequest.TopicName, trigger.MessageRequest.Message),
                new MessageResponse(true));
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        protected ClientMessage ToResponse()
        {
            return new ClientMessage
            {
                Id = Id,
                MessageResponse = new ProtoMessageResponse { Success = Res.Success },
            };
        }
    }
}
