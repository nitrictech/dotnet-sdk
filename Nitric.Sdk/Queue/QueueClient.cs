using System;
using System.Collections.Generic;
using Grpc.Core;
using Google.Protobuf.Collections;
using NitricEvent = Nitric.Proto.Common.v1.NitricEvent;
using ProtoClient = Nitric.Proto.Queue.v1.Queue.QueueClient;
using Nitric.Proto.Queue.v1;
using Nitric.Api.Common;
//using CommonEvent = Nitric.Api.Common.Event;

namespace Nitric.Api.Queue
{
    
    class QueueClient : AbstractClient
    {
        protected ProtoClient client;

        public QueueClient()
        {
            this.client = new ProtoClient(this.GetChannel());
        }

        public PushResponse SendBatch(string queue, IList<Common.Event> events)
        {
            RepeatedField<NitricEvent> wireEvents = new RepeatedField<NitricEvent>();
            foreach(Common.Event se in events)
            {
                wireEvents.Add(EventToWire(se));
            }

            var request = new QueueSendBatchRequest { Queue = queue };
            request.Events.AddRange(wireEvents);
            var response = client.SendBatch(request);
            List<FailedEvent> failedEvents = new List<FailedEvent>();
            foreach(Proto.Queue.v1.FailedEvent fe in response.FailedEvents)
            {
                failedEvents.Add(WireToFailedEvent(fe));
            }
            return new PushResponse(failedEvents);
        }

        public List<QueueItem> Receive(string queue, int depth)
        {
            if (depth < 1)
            {
                depth = 1;
            }
            var request = new QueueReceiveRequest { Queue = queue, Depth = depth };
            var response = this.client.Receive(request);
            List<QueueItem> items = new List<QueueItem>();
            foreach (NitricQueueItem nqi in response.Items)
            {
                items.Add(WireToQueueItem(nqi));
            }
            return items;
        }
        private QueueItem WireToQueueItem(NitricQueueItem nitricQueueItem)
        {
            return new QueueItem(new Common.Event(
                nitricQueueItem.Event.RequestId,
                nitricQueueItem.Event.PayloadType,
                nitricQueueItem.Event.Payload
                ), nitricQueueItem.LeaseId
            );
        }

        private NitricEvent EventToWire(Common.Event sdkEvent)
        {
            return new NitricEvent {
                RequestId = sdkEvent.RequestId,
                PayloadType = sdkEvent.PayloadType,
                Payload = sdkEvent.PayloadStruct
            };
        }

        private FailedEvent WireToFailedEvent(Proto.Queue.v1.FailedEvent protoFailedEvent)
        {
            return new FailedEvent(new Common.Event(
                protoFailedEvent.Event.RequestId,
                protoFailedEvent.Event.PayloadType,
                protoFailedEvent.Event.Payload
                ), protoFailedEvent.Message
            );
        }
        private Common.Event WireToEvent(NitricEvent nitricEvent)
        {
            return new Common.Event(
                nitricEvent.RequestId,
                nitricEvent.PayloadType,
                nitricEvent.Payload
            );
        }
    }
}