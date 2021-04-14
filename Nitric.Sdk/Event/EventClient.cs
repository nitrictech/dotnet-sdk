﻿using System;
using Nitric.Api.Common;
using NitricEvent = Nitric.Proto.Event.v1.NitricEvent;
using Nitric.Proto.Event.v1;
using ProtoClient = Nitric.Proto.Event.v1.Event.EventClient;

namespace Nitric.Api.Event
{
	public class EventClient : AbstractClient
	{
		protected ProtoClient client;

		private EventClient(ProtoClient client=null)
		{
			this.client = (client == null) ? new ProtoClient(this.GetChannel()) : client;
		}

		public string Publish(string topic, Object payload, string payloadType, string requestID)
		{
			var payloadStruct = Util.ObjectToStruct(payload);
			var evt = new NitricEvent { Id = requestID, PayloadType = payloadType, Payload = payloadStruct };
			var request = new EventPublishRequest { Topic = topic, Event = evt };

			var response = this.client.Publish(request);

			return requestID;
		}
		public class Builder
        {
			private ProtoClient client;
			//Forces the builder design pattern
			public Builder()
			{ }
			public Builder Client(ProtoClient client)
            {
				this.client = client;
				return this;
            }
			public EventClient Build()
            {
				return new EventClient(this.client);
            }

        }
	}
}