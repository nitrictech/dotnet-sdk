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

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Nitric.Proto.Topics.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Topic
{
    /// <summary>
    /// Represents a reference to a topic.
    /// </summary>
    public class Topic<T>
    {
        private readonly TopicsClient<T> TopicsClient;
        /// <summary>
        /// The name of topic.
        /// </summary>
        public string Name { get; private set; }

        internal Topic(TopicsClient<T> TopicsClient, string name)
        {
            this.TopicsClient = TopicsClient;
            this.Name = name;
        }

        /// <summary>
        /// Publish a new message to this topic.
        /// </summary>
        /// <param name="message">The message to publish</param>
        /// <exception cref="NitricException"></exception>
        public void Publish(T message)
        {
            Struct structPayload = null;
            if (message != null)
            {
                var jsonPayload = JsonConvert.SerializeObject(message);
                structPayload = JsonParser.Default.Parse<Struct>(jsonPayload);
            }

            var request = new TopicPublishRequest { TopicName = this.Name, Message = new Message { StructPayload = structPayload } };

            try
            {
                this.TopicsClient.TopicClient.Publish(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}
