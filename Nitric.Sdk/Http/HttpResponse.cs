using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Text;
using System.Linq;
namespace Nitric.Api.Http
{
    public class HttpResponse
    {
        private static readonly string ContentType = "Content-Type";

        public byte[] Body { get; private set; }
        public HttpStatusCode Status { get; private set; }
        public Dictionary<string, List<string>> Headers { get; private set; }

        /*
         * Package Private constructor to enforce response builder pattern.
         */
        private HttpResponse(byte[] body, HttpStatusCode status, Dictionary<string, List<string>> headers)
        {
            Body = body;
            Status = status;
            Headers = headers;
        }
        /**
         * Return the named response header or null if not found.
         * If the header has multiple values the first value will be returned.
         */
        public string GetHeader(string name)
        {
            var values = headers[name];
            return (values != null) ? values[0] : null;
        }
        //Gets the body's length, if the body is empty returns -1
        public int GetBodyLength()
        {
            return (body != null) ? body.Length : -1;
        }
        public string toString()
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
            private const string ContentType = "Content-Type";

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
            public HttpResponse Build()
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
                return new HttpResponse(this);
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
}
