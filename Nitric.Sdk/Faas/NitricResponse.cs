using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nitric.Api.Faas
{
    public class NitricResponse
    {
        private const string ContentType = "Content-Type";

        public readonly byte[] Body;
        public readonly HttpStatusCode Status;
        public readonly Dictionary<string, List<string>> Headers;

        // Public Methods ------------------------------------------------------------

        private NitricResponse(HttpStatusCode status,
                           Dictionary<string, List<string>> headers,
                           byte[] body)
        {
            Status = status;
            Headers = headers; //TODO: Make immutable
            Body = body;
        }
        public override string ToString()
        {
            return GetType().Name +
                    "[status=" + Status.ToString() +
                    ", headers=" + Headers +
                    ", body.length=" + ((Body != null) ? Body.Length : 0) +
                    "]";
        }
        public static Builder NewBuilder()
        {
            return new Builder();
        }
        public static NitricResponse Build(string body)
        {
            return NewBuilder().BodyText(body).Build();
        }
        public static NitricResponse Build(HttpStatusCode status)
        {
            return NewBuilder().Status(status).Build();
        }
        public static NitricResponse Build(HttpStatusCode status, string body)
        {
            return NewBuilder().Status(status).BodyText(body).Build();
        }

        public class Builder
        {
            private HttpStatusCode status;
            private Dictionary<string, List<string>> headers;
            private byte[] body;

            public Builder()
            {
                Reset();
            }
            public void Reset()
            {
                this.status = HttpStatusCode.OK;
                this.headers = new Dictionary<string, List<string>>();
                this.body = null;
            }
            //Set the function response status, e.g. 200 for HTTP OK.
            public Builder Status(HttpStatusCode status)
            {
                this.status = status;
                return this;
            }

            //Set the response header name and value.
            public Builder Header(string name, string value)
            {
                if (name == null)
                {
                    throw new ArgumentNullException("header name is required");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("header value is required");
                }
                if (headers == null)
                {
                    headers = new Dictionary<string, List<string>>();
                }
                headers.Add(name, value.Split(',').ToList());
                return this;
            }

            //Set the function response headers
            public Builder Headers(Dictionary<string, List<string>> headers)
            {
                this.headers = headers;
                return this;
            }
            //Overload that accepts a dictionary of just string, string
            public Builder Headers(Dictionary<string, string> headers)
            {
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                foreach(KeyValuePair<string, string> entry in headers)
                {
                    dict.Add(entry.Key, entry.Value.Split(',').ToList());
                }
                this.headers = dict;
                return this;
            }

            //Set the function response body.
            public Builder Body(byte[] body)
            {
                this.body = body;
                return this;
            }
            
            //Set the function response body text (UTF-8) encoded.
            public Builder BodyText(string body)
            {
                this.body = (body != null) ? Encoding.UTF8.GetBytes(body.ToCharArray()) : null;
                return this;
            }

            //Return a new function response object
            public NitricResponse Build()
            {
                Dictionary<string, List<string>> responseHeaders =
                (headers != null) ? headers : new Dictionary<string, List<string>>();
                if (GetHeaderValue(ContentType, responseHeaders) == null)
                {
                    var contentType = DetectContentType(body);
                    if (contentType != null)
                    {
                        responseHeaders.Add(ContentType, contentType.Split(',').ToList());
                    }
                }
                return new NitricResponse(status, responseHeaders, body);
            }
            
            // Private Methods ------------------------------------------------------------

            private string DetectContentType(byte[] body)
            {
                if (body != null && body.Length > 1)
                {
                    var bodyText = Encoding.UTF8.GetString(body);
                    if ((bodyText.StartsWith("{") && bodyText.EndsWith("}"))
                       || (bodyText.StartsWith("[") && bodyText.EndsWith("]")))
                    {
                        return "text/json; charset=UTF-8";
                    }
                    if (bodyText.StartsWith("<?xml") && bodyText.EndsWith(">"))
                    {
                        return "text/xml; charset=UTF-8";
                    }
                    return "text/html; charset=UTF-8";
                }
                return null;
            }
            private string GetHeaderValue(string name, Dictionary<string, List<string>> headers)
            {
                foreach (KeyValuePair<string, List<string>> entry in headers)
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
