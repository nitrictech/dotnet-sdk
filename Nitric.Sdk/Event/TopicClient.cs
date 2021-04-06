using System;
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
        private TopicClient()
        {
            this.client = new ProtoClient(this.GetChannel());
        }
		public List<Topic> List()
        {
			var request = new TopicListRequest { };

			var response = client.List(request);

            List<Topic> topics = new List<Topic>();
			foreach (NitricTopic r in response.Topics)
            {
                new Topic
                    .Builder()
                    .Build(r.Name);
            }
            return topics;
		}
        public class Builder
        {
            //Forces the builder design pattern
            public Builder()
            { }
            public TopicClient Build()
            {
                return new TopicClient();
            }
        }
    }
}
