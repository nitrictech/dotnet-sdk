using System.Collections;
using Proto = Nitric.Client.Grpc.v1;
using Grpc.Core;
using NitricEvent = Nitric.Client.Grpc.v1.NitricEvent;
using Util = Nitric.Sdk.v1.Util.Util;

namespace Nitric.Sdk.Event.V1
{
	public class EventClient
	{
		

		public EventClient(Grpc.GrpcClient conn)
		{
			new Proto.Event.v1.Event.EventClient();
			// TODO: Construct the channel using shared code (for all clients). Pull host details from environment variables.
			Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);
			this.client = new BaseClient.EventClient(channel);
		}

		public string Publish(string topic, Object payload, string payloadType, string requestID)
		{
			Struct payloadStruct = Util.DictToMessage(payload);
			var evt = new NitricEvent { RequestID = requestID, PayloadType = payloadType, Payload = payloadStruct };
			this.client.Publish(new EventPublishRequest { Topic = topic, Event = evt });
			this.client.Publish(new EventPublishRequest { Topic = topic, Event = evt });
		}
	}
}