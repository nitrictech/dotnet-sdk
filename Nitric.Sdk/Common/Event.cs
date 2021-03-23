using Google.Protobuf.WellKnownTypes;

namespace Nitric.Api.Common
{
    public class Event
    {
        public string RequestId { get; set; }
        public string PayloadType { get; set; }
        public Struct PayloadStruct { get; set; }
        public Event(string requestId, string payloadType, Struct payloadLoadStruct)
        {
            RequestId = requestId;
            PayloadType = payloadType;
            PayloadStruct = payloadLoadStruct;
        }
    }
}
