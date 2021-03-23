using System;
using Grpc.Core;
using Nitric.Sdk.Common.v1;
using NitricEvent = Nitric.Proto.Common.v1.NitricEvent;
using Nitric.Proto.Event.v1;
using ProtoClient = Nitric.Proto.Event.v1.Event.EventClient;

namespace Nitric.Api.Event
{
	public class EventClient : AbstractClient
	{
		protected ProtoClient client;
		
		public EventClient()
        {
			this.client = new ProtoClient(this.GetChannel());
        }

		public string Publish(string topic, Object payload, string payloadType, string requestID)
		{
			// TODO: Remove once request id generation has been moved to the Nitric Membrane.
			if (string.IsNullOrEmpty(requestID))
            {
				requestID = Guid.NewGuid().ToString();
            }

			var payloadStruct = Util.ObjectToStruct(payload);
			var evt = new NitricEvent { RequestId = requestID, PayloadType = payloadType, Payload = payloadStruct };
			var request = new EventPublishRequest { Topic = topic, Event = evt };

			this.client.Publish(request);

			return requestID;
		}
	}
}