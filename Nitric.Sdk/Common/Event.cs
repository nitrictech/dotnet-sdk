using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
namespace Nitric.Api.Common
{
    public class Event
    {
        public string RequestId { get; protected set; }
        public string PayloadType { get; protected set; }
        public Struct Payload { get; protected set; }
        private Event(string requestId, string payloadType, Struct payload)
        {
            Payload = payload;
            RequestId = requestId;
            PayloadType = payloadType;
        }
        public override string ToString()
        {
            return GetType().Name+
                    "[id=" + RequestId
                    + ", payloadType=" + PayloadType
                    + ", payload=" + Payload
                    + "]";
        }
        public class Builder
        {
            private string requestId;
            private string payloadType;
            private Struct payload;

            public Builder()
            {
                this.requestId = "";
                this.payloadType = "";
                this.payload = Util.ObjectToStruct(new Dictionary<string,string>());
            }
            public Builder RequestId(string requestId)
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
            public Event Build()
            {
                return new Event(requestId, payloadType, payload);
            }
        }
    }
}
