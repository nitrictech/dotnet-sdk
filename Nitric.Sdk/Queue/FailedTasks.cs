using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
namespace Nitric.Api.Queue
{
    public class FailedTask
    {
        public string Message { get; private set; }
        public string RequestId { get; private set; }
        public string PayloadType { get; private set; }
        public Struct PayloadStruct { get; private set; }
        private FailedTask(
            string requestId,
            string payloadType,
            Struct payloadStruct,
            string message)
        {
            this.Message = message;
            this.RequestId = requestId;
            this.PayloadType = payloadType;
            this.PayloadStruct = (payloadStruct == null) ?
                Common.Util.ObjectToStruct(new Dictionary<string,string>()) :
                payloadStruct;
        }

        public class Builder
        {
            private string requestId;
            private string payloadId;
            private Struct payloadStruct;
            private string message;
            //Defines the defaults
            public Builder()
            {
                this.requestId = null;
                this.payloadId = null;
                this.payloadStruct = null;
                this.message = null;
            }
            public Builder RequestId(string requestId)
            {
                this.requestId = requestId;
                return this;
            }
            public Builder PayloadId(string payloadId)
            {
                this.payloadId = payloadId;
                return this;
            }
            public Builder PayloadStruct(Struct payloadStruct)
            {
                this.payloadStruct = payloadStruct;
                return this;
            }
            public Builder Message(string message)
            {
                this.message = message;
                return this;
            }
            public FailedTask Build()
            {
                return new FailedTask(requestId, payloadId, payloadStruct, message);
            }
        }
    }
}
