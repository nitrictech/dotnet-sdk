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

using Nitric.Proto.Schedules.v1;

namespace Nitric.Sdk.Service
{
    /// <summary>
    /// Represents a message pushed to a subscriber via a topic.
    /// </summary>
    public class IntervalRequest : TriggerRequest
    {
        /// <summary>
        /// The name of the schedule that triggered this request
        /// </summary>
        public string ScheduleName { get; private set; }

        /// <summary>
        /// Construct an interval request
        /// </summary>
        /// <param name="scheduleName">the source schedule</param>
        public IntervalRequest(string scheduleName) : base()
        {
            this.ScheduleName = scheduleName;
        }
    }

    /// <summary>
    /// Represents the results of processing an event.
    /// </summary>
    public class IntervalResponse : TriggerResponse { }

    /// <summary>
    /// Represents the request/response context for an event.
    /// </summary>
    public class IntervalContext : TriggerContext<IntervalRequest, IntervalResponse>
    {
        /// <summary>
        /// Construct a new IntervalContext.
        /// </summary>
        /// <param name="req">The request object</param>
        /// <param name="res">The response object</param>
        public IntervalContext(string id, IntervalRequest req, IntervalResponse res) : base(id, req, res)
        {
        }

        /// <summary>
        /// Construct an event topic from a trigger request gRPC object.
        /// </summary>
        /// <param name="trigger">The trigger to convert into an EventContext.</param>
        /// <returns>the new event context</returns>
        internal static IntervalContext FromRequest(ServerMessage trigger)
        {
            return new IntervalContext(trigger.Id, new IntervalRequest(trigger.IntervalRequest.ScheduleName),
                new IntervalResponse());
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        internal ClientMessage ToResponse()
        {
            return new ClientMessage { Id = Id, IntervalResponse = new Proto.Schedules.v1.IntervalResponse() };
        }
    }
}
