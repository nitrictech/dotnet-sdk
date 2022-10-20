using Nitric.Proto.Event.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Common.Util;

namespace Nitric.Sdk.Event
{
    /// <summary>
    /// Represents a reference to a topic.
    /// </summary>
    public class Topic
    {
        private readonly EventsClient eventsClient;
        /// <summary>
        /// The name of topic.
        /// </summary>
        public string Name { get; private set; }

        internal Topic(EventsClient eventsClient, string name)
        {
            this.eventsClient = eventsClient;
            this.Name = name;
        }

        private static NitricEvent EventToWire(Event evt)
        {
            return new NitricEvent
            {
                Id = evt.Id ?? "",
                PayloadType = evt.PayloadType ?? "",
                Payload = Utils.ObjToStruct(evt.Payload)
            };
        }

        /// <summary>
        /// Publish a new event to this topic.
        /// </summary>
        /// <param name="evt">The event to publish</param>
        /// <returns>The unique id of the published event.</returns>
        /// <exception cref="NitricException"></exception>
        public string Publish(Event evt)
        {
            var payloadStruct = Utils.ObjToStruct(evt.Payload);
            var nEvt = EventToWire(evt);
            var request = new EventPublishRequest { Topic = this.Name, Event = nEvt };

            try
            {
                var response = this.eventsClient.EventClient.Publish(request);
                return response.Id;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }
}
