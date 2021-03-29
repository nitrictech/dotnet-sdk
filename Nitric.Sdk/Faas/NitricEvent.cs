using System;
using System.Collections.Generic;
namespace Nitric.Api.Faas
{
    public class NitricEvent
    {
        public NitricContext Context { get; private set; }
        public byte[] Payload { get; private set; }

        public NitricEvent(NitricContext context, byte[] payload)
        {
            Context = context;
            Payload = payload;
        }
        public static NitricEvent.Builder NewBuilder()
        {
            return new NitricEvent.Builder();
        }
        //String representation of this object
        public string toString()
        {
            return GetType().Name +
                    "[context=" + Context +
                    ", payload.length=" + ((Payload != null) ? Payload.Length : 0) +
                    "]";
        }
        public class NitricContext
        {
            private string requestID;
            private string source;
            private string sourceType;
            private string payloadType;
            public NitricContext(string requestID = null, string source = null, string sourceType = null, string payloadType = null)
            {
                this.requestID = requestID;
                this.source = source;
                this.sourceType = sourceType;
                this.payloadType = payloadType;
            }
            public static string CleanHeader(string headerName)
            {
                return headerName.ToLower().Replace("x-nitric-", "").Replace("-", "_");
            }
            public string toString()
            {
                return GetType().ToString() +
                        "[requestId=" + requestID
                        + ",sourceType=" + sourceType
                        + ",source=" + source
                        + ",payloadType=" + payloadType
                        + "]";
            }
        }
        public class Builder
        {
            private Dictionary<string, List<string>> headers;
            private byte[] payload;

            public Builder()
            {
                this.Reset();
            }
            public void Reset()
            {
                this.headers = null;
                this.payload = null;
            }

            //Set the event headers.
            public Builder Headers(Dictionary<string, List<string>> headers)
            {
                this.headers = headers;
                return this;
            }

            //Set the event payload.
            public Builder Payload(byte[] payload)
            {
                this.payload = payload;
                return this;
            }

            //returns a new Nitric Event.
            public NitricEvent build()
            {
                var requestID = GetHeaderValue("x-nitric-request-id", headers);
                var sourceType = GetHeaderValue("x-nitric-source-type", headers);
                var source = GetHeaderValue("x-nitric-source", headers);
                var payloadType = GetHeaderValue("x-nitric-payload-type", headers);

                var context = new NitricContext(requestID, sourceType, source, payloadType);

                return new NitricEvent(context, payload);
            }

            // Private Methods ------------------------------------------------------------

            private static string GetHeaderValue(String name, Dictionary<string, List<string>> headers)
            {
                if (headers == null || headers.Count == 0)
                {
                    return null;
                }
                foreach(KeyValuePair<string, List<string>> entry in headers)
                {
                    if (entry.Key.ToLower() == name.ToLower())
                    {
                        return entry.Value[0];
                    }
                }
                return null;
            }
        }
    }
}
