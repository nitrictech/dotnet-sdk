using System.Collections.Generic;

namespace Nitric.Faas
{
    public class NitricEvent
    {
        public NitricContext Context { get; private set; }
        public byte[] Payload { get; private set; }

        private NitricEvent(NitricContext context, byte[] payload)
        {
            this.Context = context;
            this.Payload = payload;
        }

        public class NitricContext
        {
            //The event requestID
            public string RequestID { get; private set; }

            //The event source type
            public string SourceType { get; private set; }

            //The event source
            public string Source { get; private set; }

            //The event payloadType
            public string PayloadType { get; private set; }

            public NitricContext(string requestID, string sourceType, string source, string payloadType)
            {
                this.RequestID = requestID;
                this.SourceType = sourceType;
                this.Source = source;
                this.PayloadType = payloadType;
            }
            public string toString()
            {
                return GetType().Name
                    + "[requestId=" + this.RequestID
                    + ", sourceType=" + this.SourceType
                    + ", source=" + this.Source
                    + ", payloadType=" + this.PayloadType
                    + "]";
            }
        }
        public class Builder
        {
            private Dictionary<string, List<string>> headers;
            private byte[] payload;

            public Builder()
            {
                this.headers = new Dictionary<string, List<string>>();
                this.payload = null;
            }

            public Builder Headers(Dictionary<string, List<string>> headers)
            {
                this.headers = headers;
                return this;
            }

            public Builder Payload(byte[] payload)
            {
                this.payload = payload;
                return this;
            }

            public NitricEvent Build()
            {
                var requestID = HeaderValue("x-nitric-request-id", headers);
                var sourceType = HeaderValue("x-nitric-source-type", headers);
                var source = HeaderValue("x-nitric-source", headers);
                var payloadType = HeaderValue("x-nitric-payload-type", headers);

                var context = new NitricContext(requestID, sourceType, source, payloadType);

                return new NitricEvent(context, payload);
            }

            private static string HeaderValue(string name, Dictionary<string, List<string>> headers)
            {
                if (headers == null || headers.Count == 0)
                {
                    return null;
                }
                foreach(KeyValuePair<string, List<string>> kv in headers)
                {
                    if (kv.Key.ToLower() == name.ToLower())
                    {
                        return kv.Value[0];
                    }
                }
                return null;
            }
        }
    }
}
