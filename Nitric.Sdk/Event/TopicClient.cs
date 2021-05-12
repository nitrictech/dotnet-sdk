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

        public static Builder NewBuilder() {
            return new Builder();
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
