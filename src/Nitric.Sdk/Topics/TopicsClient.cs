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
using Nitric.Sdk.Common;
using TopicsGrpcClient = Nitric.Proto.Topics.v1.Topics.TopicsClient;

namespace Nitric.Sdk.Topics
{
    /// <summary>
    /// Events service client.
    /// </summary>
    public class TopicsClient<T>
    {
        internal readonly TopicsGrpcClient Client;

        /// <summary>
        /// Create a new events service client.
        /// </summary>
        /// <param name="topic">The topics gRPC client.</param>
        public TopicsClient(TopicsGrpcClient topic = null)
        {
            this.Client = topic ?? new TopicsGrpcClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to a topic.
        /// </summary>
        /// <param name="topicName">The name of the topic.</param>
        /// <returns>The topic reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Topic<T> Topic(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new ArgumentNullException(nameof(topicName));
            }

            return new Topic<T>(this, topicName);
        }
    }
}
