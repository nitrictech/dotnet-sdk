using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
namespace Nitric.Api.Common
{
    public class Event
    {
        public string RequestId { get; protected set; }
        public string PayloadType { get; protected set; }
        public Struct PayloadStruct { get; protected set; }
        private Event(string requestId, string payloadType, Struct payloadStruct)
        {
            PayloadStruct = payloadStruct;
            RequestId = requestId;
            PayloadType = payloadType;
        }
        public override string ToString()
        {
            return GetType().Name+
                    "[id=" + RequestId
                    + ", payloadType=" + PayloadType
                    + ", payload=" + PayloadStruct
                    + "]";
        }
        public class Builder
        {
            private string requestId;
            private string payloadType;
            private Struct payloadStruct;

            public Builder()
            {
                this.requestId = "";
                this.payloadType = "";
                this.payloadStruct = Common.Util.ObjectToStruct(new Dictionary<string,string>());
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
            public Builder PayloadStruct(Struct payloadStruct)
            {
                this.payloadStruct = payloadStruct;
                return this;
            }
            public Event Build()
            {
                return new Event(requestId, payloadType, payloadStruct);
            }
        }
    }
}
