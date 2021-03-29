using System.Collections.Generic;
using System.Web;
using System.Text;

namespace Nitric.Api.Faas
{
    public class NitricRequest
    {
        private const string ContentType = "Content-Type";

        public string Path { get; private set; }
        public Dictionary<string, List<string>> Headers { get; private set; }
        public byte[] Body { get; private set; }
        public Dictionary<string, List<string>> Parameters { get; private set; }

        // Public Methods ------------------------------------------------------------

        public NitricRequest(string path,
                          Dictionary<string, List<string>> headers,
                          byte[] body,
                          Dictionary<string, List<string>> parameters)
        {
            Path = path;
            Headers = headers;
            Body = body;
            Parameters = parameters;
        }
        public static Builder newBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private const string ContentType = "Content-Type";
            private const string FormUrlEncoded = "application/x-www-form-urlencoded";

            private string method;
            private string path;
            private string query;
            private Dictionary<string, List<string>> headers;
            private byte[] body;

            public Builder()
            {
                Reset();
            }
            public void Reset()
            {
                this.method = null;
                this.path = null;
                this.query = null;
                this.headers = new Dictionary<string, List<string>>();
                this.body = null;
            }

            // Public Methods ------------------------------------------------------------

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

            //Set the request headers
            public Builder Headers(Dictionary<string, List<string>> headers)
            {
                this.headers = headers;
                return this;
            }

            //Set the request body.
            public Builder Body(byte[] body)
            {
                this.body = body;
                return this;
            }

            //returns a new Nitric response.
            public NitricRequest Build()
            {
                //TODO: Add immutable dictionary if headers != null, otherwise empty dictionary
                var immutableHeaders = new Dictionary<string, List<string>>();

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

                return new NitricRequest(path, immutableHeaders, body, parameters);
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
