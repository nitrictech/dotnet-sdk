using System.Collections;
using BaseClient = Nitric.Client.Grpc.v1;
using NitricEvent = Nitric.Client.Grpc.v1.NitricEvent;
using Model = Nitric.Sdk.v1.Model;

namespace Nitric.Sdk.Queue.V1
{
    class PushResponse
    {
        private ArrayList failedEvents;
        public PushResponse(ArrayList failedEvents)
        {
            this.failedEvents = failedEvents;
        }
        public ArrayList getFailedEvents()
        {
            return failedEvents;
        }
    }
    class QueueClient
    {
        public QueueClient()
        {

        }
        public NitricEvent eventToWire(Model.Event nitricEvent)
        {
            return NitricEvent(

            );
        }
        public Model.Event wireToEvent(NitricEvent nitricEvent)
        {
            return Model.Event(
                nitricEvent.requestId,
                nitricEvent.payloadType,
                MessageToDict(nitricEvent.payload)
            );
        }
    }
}