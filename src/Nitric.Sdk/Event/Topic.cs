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

using Nitric.Proto.Event.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Event
{
    /// <summary>
    /// Represents a reference to a topic.
    /// </summary>
    public class Topic
    {
        private readonly EventsClient eventsClient;
        /// <summary>
        /// The name of topic.
        /// </summary>
        public string Name { get; private set; }

        internal Topic(EventsClient eventsClient, string name)
        {
            this.eventsClient = eventsClient;
            this.Name = name;
        }

        /// <summary>
        /// Publish a new event to this topic.
        /// </summary>
        /// <param name="evt">The event to publish</param>
        /// <returns>The unique id of the published event.</returns>
        /// <exception cref="NitricException"></exception>
        public string Publish(Event evt)
        {
            var request = new EventPublishRequest { Topic = this.Name, Event = evt.ToWire() };

            try
            {
                var response = this.eventsClient.EventClient.Publish(request);
                return response.Id;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}
