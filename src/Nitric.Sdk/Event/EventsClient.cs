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
using System.Collections.Generic;
using System.Linq;
using Nitric.Sdk.Common;
using NitricEvent = Nitric.Proto.Event.v1.NitricEvent;
using Nitric.Proto.Event.v1;
using Nitric.Sdk.Common.Util;
using GrpcClient = Nitric.Proto.Event.v1.EventService.EventServiceClient;
using TopicClient = Nitric.Proto.Event.v1.TopicService.TopicServiceClient;

namespace Nitric.Sdk.Event
{
    /// <summary>
    /// Events service client.
    /// </summary>
    public class EventsClient
    {
        internal readonly GrpcClient EventClient;
        private readonly TopicClient topicClient;

        /// <summary>
        /// Create a new events service client.
        /// </summary>
        /// <param name="client">The events gRPC client.</param>
        /// <param name="topic">The topics gRPC client.</param>
        public EventsClient(GrpcClient client = null, TopicClient topic = null)
        {
            this.EventClient = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
            this.topicClient = topic ?? new TopicClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to a topic.
        /// </summary>
        /// <param name="topicName">The name of the topic.</param>
        /// <returns>The topic reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Topic Topic(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new ArgumentNullException(nameof(topicName));
            }

            return new Topic(this, topicName);
        }

        /// <summary>
        /// Return a list of all accessible topics.
        /// </summary>
        /// <returns>A list of accessible topics.</returns>
        /// <exception cref="NitricException"></exception>
        public List<Topic> List()
        {
            var request = new TopicListRequest { };

            try
            {
                var response = topicClient.List(request);
                return response.Topics.Select(topic => new Topic(this, topic.Name)).ToList();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}
