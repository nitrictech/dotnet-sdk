﻿using System;
using Nitric.Api.Common;
using NitricTopic = Nitric.Proto.Event.v1.NitricTopic;
using Nitric.Proto.Event.v1;
using ProtoClient = Nitric.Proto.Event.v1.Topic.TopicClient;
using System.Collections.Generic;
namespace Nitric.Api.Event
{
    public class TopicClient : AbstractClient
    {
		protected ProtoClient client;
        private TopicClient(ProtoClient client)
        {
            this.client = (client == null) ? new ProtoClient(this.GetChannel()) : client;
        }
		public List<Topic> List()
        {
			var request = new TopicListRequest { };

			var response = client.List(request);

            List<Topic> topics = new List<Topic>();
			foreach (NitricTopic r in response.Topics)
            {
                topics.Add(new Topic
                    .Builder()
                    .Build(r.Name));
            }
            return topics;
		}
        public class Builder
        {
            private ProtoClient client;
            //Forces the builder design pattern
            public Builder()
            {
                this.client = null;
            }
            public Builder Client(ProtoClient client)
            {
                this.client = client;
                return this;
            }
            public TopicClient Build()
            {
                return new TopicClient(client);
            }
        }
    }
}
