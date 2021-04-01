using System.Collections.Generic;
using Google.Protobuf.Collections;
using NitricEvent = Nitric.Proto.Event.v1.NitricEvent;
using ProtoClient = Nitric.Proto.Queue.v1.Queue.QueueClient;
using Nitric.Proto.Queue.v1;
using Nitric.Api.Common;

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
            RepeatedField<NitricTask> wireEvents = new RepeatedField<NitricTask>();
            foreach(Common.Event se in events)
            {
                wireEvents.Add(EventToWire(se));
            }

            var request = new QueueSendBatchRequest { Queue = queue };
            request.Tasks.AddRange(wireEvents);
            var response = client.SendBatch(request);
            List<FailedTask> failedTasks = new List<FailedTask>();
            foreach(Proto.Queue.v1.FailedTask fe in response.FailedTasks)
            {
                failedTasks.Add(WireToFailedEvent(fe));
            }
            return new PushResponse(failedTasks);
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
            foreach (NitricTask nqi in response.Tasks)
            {
                items.Add(WireToQueueItem(nqi));
            }
            return items;
        }
        private QueueItem WireToQueueItem(NitricTask nitricTask)
        {
            return new QueueItem(new Common.Event(
                nitricTask.Id,
                nitricTask.PayloadType,
                nitricTask.Payload),
                nitricTask.LeaseId
            );
        }

        private NitricTask EventToWire(Common.Event sdkEvent)
        {
            return new NitricTask {
                Id = sdkEvent.RequestId,
                PayloadType = sdkEvent.PayloadType,
                Payload = sdkEvent.PayloadStruct
            };
        }

        private FailedTask WireToFailedEvent(Proto.Queue.v1.FailedTask protoFailedEvent)
        {
            return new FailedTask.Builder()
                .RequestId(protoFailedEvent.Task.Id)
                .PayloadId(protoFailedEvent.Task.PayloadType)
                .PayloadStruct(protoFailedEvent.Task.Payload)
                .Message(protoFailedEvent.Message)
                .Build();
        }
        private Common.Event WireToEvent(NitricTask nitricEvent)
        {
            return new Common.Event(
                nitricEvent.Id,
                nitricEvent.PayloadType,
                nitricEvent.Payload
            );
        }
    }
}