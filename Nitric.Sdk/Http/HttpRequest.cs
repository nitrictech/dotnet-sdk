using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace Nitric.Api.Http
{
    public class HttpRequest
    {
        public string Method { get; }
        public string Path { get; }
        public string Query { get; }
        public Dictionary<string, List<string>> Headers { get; }
        public byte[] Body { get; }
        public string BodyText
        {
            get { return Encoding.UTF8.GetString(this.Body); }
            private set { }
        }
        public Dictionary<string, List<string>> Parameters { get; }
        public HttpRequest(string method,
                           string path,
                           string query,
                           Dictionary<string, List<string>> headers,
                           byte[] body,
                           Dictionary<string, List<string>> parameters)
        {
            Method = method;
            Path = path;
            Query = query;
            Headers = headers;
            Body = body;
            Parameters = parameters;
        }
        public string toString()
        {
            return GetType().Name +
                    "[method=" + Method +
                    ", path=" + Path +
                    ", query=" + Query +
                    ", headers=" + Headers +
                    ", parameters=" + Parameters +
                    ", body.length=" + ((Body != null) ? Body.Length : 0) +
                    "]";
        }

        public static HttpRequest.Builder NewBuilder()
        {
            return new HttpRequest.Builder();
        }

        public class Builder
        {
            private const string ContentType = "Content-Type";
            private const string FormUrlEncoded = "application/x-www-form-urlencoded";

            string method;
            string path;
            string query;
            Dictionary<string, List<string>> headers;
            Dictionary<string, List<string>> parameters;
            byte[] body;

            public Builder()
            {
                this.method = "";
                this.path = "";
                this.query = "";
                this.headers = new Dictionary<string, List<string>>();
                this.parameters = new Dictionary<string, List<string>>();
                this.body = null;
            }
            //Set the request method, for example with HTTP this would be ["GET" | "POST" | "PUT" | "DELETE" ].
            public Builder Method(string method)
            {
                this.method = method;
                return this;
            }

            //Set the request path.
            public Builder Path(string path)
            {
                this.path = path;
                return this;
            }

            //Set the request URL query.
            public Builder Query(string query)
            {
                this.query = query;
                return this;
            }

            //Set the request headers.
            public Builder Headers(Dictionary<string, List<string>> headers)
            {
                this.headers = headers;
                return this;
            }

            // TODO: multi-part file posts

            //Set the request body.
            public Builder Body(byte[] body)
            {
                this.body = body;
                return this;
            }
            public Builder Body(string body)
            {
                this.body = (body != null) ? Encoding.UTF8.GetBytes(body) : null;
                return this;
            }
            //returns a new Http request.
            public HttpRequest Build()
            {
                //TODO: Add immutable dictionary if headers != null, otherwise empty dictionary
                var immutableHeaders = new Dictionary<string, List<string>>();
                this.method = (method == null) ? "" : this.method;
                string urlParameters = null;
                if ("POST".ToLower() == method.ToLower())
                {
                    var contentType = GetHeaderValue(ContentType, headers);
                    if (body != null && body.Length > 0 && FormUrlEncoded.ToLower() == contentType.ToLower())
                    {
                        urlParameters = HttpUtility.UrlDecode(Encoding.UTF8.GetString(body));
                    }
                }
                else if (query != null)
                {
                    urlParameters = query;
                }
                Dictionary<string, List<string>> parameters = ParseParameters(urlParameters);

                return new HttpRequest(method, path, query, headers, body, parameters);
            }

            // Private Methods ------------------------------------------------------------

            private static string GetHeaderValue(string name, Dictionary<string, List<string>> headers)
            {
                if (headers == null || headers.Count == 0)
                {
                    return null;
                }
                foreach (KeyValuePair<string, List<string>> entry in headers)
                {
                    if (entry.Key.ToLower() == name.ToLower())
                    {
                        return entry.Value[0];
                    }
                }

                return null;
            }

            private static Dictionary<string, List<string>> ParseParameters(string urlParameters)
            {
                Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();

                if (urlParameters != null && urlParameters.Length > 2)
                {
                    string[] urlParams = urlParameters.Split('&');

                    foreach (string s in urlParams)
                    {
                        var paramArray = s.Split('=');
                        if (paramArray.Length > 1)
                        {
                            var name = paramArray[0];
                            var value = paramArray[2];
                            var valueList = parameters[name];
                            if (parameters[name] != null)
                            {
                                valueList = new List<string>();
                                parameters.Add(name, valueList);
                            }
                            valueList.Add(value);
                        }
                    }
                }
                return parameters;
            }
        }
    }
}
