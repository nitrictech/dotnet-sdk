using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
namespace Nitric.Api.Queue
{
    public class Task
    {
        public Common.Event Event { get; private set; }
        public string LeaseID { get; private set; }
        private Task(string requestId,
                          string payloadType,
                          Struct payload,
                          string leaseID)
        {
            Event = new Common.Event.Builder()
                .RequestId(requestId)
                .PayloadType(payloadType)
                .Payload(payload)
                .Build();
            LeaseID = leaseID;
        }
        public override string ToString()
        {
            return GetType().Name + "[event=" + Event + ", leaseId=" + LeaseID + "]";
        }
        public class Builder
        {
            private string requestId;
            private string payloadType;
            private Struct payload;
            private string leaseID;
            public Builder()
            {
                this.requestId = null;
                this.payloadType = null;
                this.payload = Common.Util.ObjectToStruct(new Dictionary<string, string>());
                this.leaseID = null;
            }
            public Builder RequestID(string requestId)
            {
                this.requestId = requestId;
                return this;
            }
            public Builder PayloadType(string payloadType)
            {
                this.payloadType = payloadType;
                return this;
            }
            public Builder Payload(Struct payload)
            {
                this.payload = payload;
                return this;
            }
            public Builder LeaseID(string leaseID)
            {
                this.leaseID = leaseID;
                return this;
            }
            public Task Build()
            {
                return new Task(requestId, payloadType, payload, leaseID);
            }
        }
    }
}
