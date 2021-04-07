using System.Collections.Generic;
using Google.Protobuf.Collections;
using NitricEvent = Nitric.Proto.Event.v1.NitricEvent;
using ProtoClient = Nitric.Proto.Queue.v1.Queue.QueueClient;
using Nitric.Proto.Queue.v1;
using Nitric.Api.Common;

namespace Nitric.Api.Queue
{
    
    public class QueueClient : AbstractClient
    {
        protected ProtoClient client;

        private QueueClient(ProtoClient client)
        {
            this.client = (client == null) ? new ProtoClient(this.GetChannel()) : client;
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
            return new QueueItem
                .Builder()
                .RequestID(nitricTask.Id)
                .PayloadType(nitricTask.PayloadType)
                .Payload(nitricTask.Payload)
                .LeaseID(nitricTask.LeaseId)
                .Build();
        }

        private NitricTask EventToWire(Common.Event sdkEvent)
        {
            return new NitricTask {
                Id = sdkEvent.RequestId,
                PayloadType = sdkEvent.PayloadType,
                Payload = sdkEvent.Payload
            };
        }

        private FailedTask WireToFailedEvent(Proto.Queue.v1.FailedTask protoFailedEvent)
        {
            return new FailedTask.Builder()
                .RequestId(protoFailedEvent.Task.Id)
                .PayloadType(protoFailedEvent.Task.PayloadType)
                .Payload(protoFailedEvent.Task.Payload)
                .Message(protoFailedEvent.Message)
                .Build();
        }
        private Common.Event WireToEvent(NitricTask nitricTask)
        {
            return new Common.Event
                .Builder()
                .RequestId(nitricTask.Id)
                .PayloadType(nitricTask.PayloadType)
                .Payload(nitricTask.Payload)
                .Build();
        }
        public class Builder
        {
            private ProtoClient client;
            public Builder()
            {
                client = null;
            }
            public Builder Client(ProtoClient client)
            {
                this.client = client;
                return this;
            }
            public QueueClient Build()
            {
                return new QueueClient(client);
            }
        }
    }
}