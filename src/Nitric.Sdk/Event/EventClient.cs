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
using Nitric.Sdk.Common;
using NitricEvent = Nitric.Proto.Event.v1.NitricEvent;
using Nitric.Proto.Event.v1;
using Nitric.Sdk.Common.Util;
using GrpcClient = Nitric.Proto.Event.v1.EventService.EventServiceClient;
using TopicClient = Nitric.Proto.Event.v1.TopicService.TopicServiceClient;

namespace Nitric.Sdk.Event
{
    public class Events : AbstractClient
    {
        internal GrpcClient EventClient;
        internal TopicClient TopicClient;

        public Events(GrpcClient client = null, TopicClient topic = null)
        {
            this.EventClient = (client == null) ? new GrpcClient(this.GetChannel()) : client;
            this.TopicClient = (topic == null) ? new TopicClient(this.GetChannel()) : topic;
        }

        public Topic Topic(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new ArgumentNullException("topicName");
            }
            return new Topic(this, topicName);
        }
        public List<Topic> List()
        {
            var request = new TopicListRequest { };

            try
            {
                var response = TopicClient.List(request);
                List<Topic> topics = new List<Topic>();
                foreach (NitricTopic topic in response.Topics)
                {
                    topics.Add(new Topic(this, topic.Name));
                }
                return topics;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
    public class Topic : AbstractClient
    {
        internal Events Events;
        public string Name { get; private set; }

        internal Topic(Events events, string name, TopicClient topic = null)
        {
            this.Events = events;
            this.Name = name;
        }
        public string Publish(Event evt)
        {
            var payloadStruct = Utils.ObjToStruct(evt.Payload);
            var nEvt = new NitricEvent { Id = evt.Id, PayloadType = evt.PayloadType, Payload = payloadStruct };
            var request = new EventPublishRequest { Topic = this.Name, Event = nEvt };

            try
            {
                var response = this.Events.EventClient.Publish(request);
                return response.Id;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}
