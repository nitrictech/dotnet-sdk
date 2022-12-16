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

using System;

namespace Nitric.Sdk.Event
{
    /// <summary>
    /// Events represent a Message delivered via Publish/Subscribe.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The unique ID of this event.
        ///
        /// Used to every reprocessing.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The expected type of the event's payload.
        /// </summary>
        public string PayloadType { get; set; }

        /// <summary>
        /// The payload (contents) of the event.
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Create a new event.
        /// </summary>
        public Event()
        {
        }

        /// <summary>
        /// Create a new event object.
        /// </summary>
        /// <param name="id">The unique id for this event.</param>
        /// <param name="payloadType">The type of this event's payload.</param>
        /// <param name="payload">The contents of the event message.</param>
        public Event(string id, string payloadType, object payload)
        {
            this.Payload = payload;
            this.Id = id;
            this.PayloadType = payloadType;
        }

        /// <summary>
        /// Return a string representing the details of the event.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return GetType().Name +
                   "[id=" + Id
                   + ", payloadType=" + PayloadType
                   + ", payload=" + Payload
                   + "]";
        }
    }
}
