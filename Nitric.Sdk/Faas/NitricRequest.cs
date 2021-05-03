// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
﻿using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Linq;

namespace Nitric.Api.Faas
{
    public class NitricRequest
    {
        public string Path { get; }
        public Dictionary<string, List<string>> Headers { get; }
        public byte[] Body { get; }
        public Dictionary<string, List<string>> Parameters { get; }
        public string BodyText {
            get { return Encoding.UTF8.GetString(this.Body); }
        }

        // Public Methods ------------------------------------------------------------

        private NitricRequest(string path,
                          Dictionary<string, List<string>> headers,
                          byte[] body,
                          Dictionary<string, List<string>> parameters)
        {
            Path = path;
            Headers = headers;
            Body = body;
            Parameters = parameters;
        }

        public override string ToString()
        {
            return GetType().Name +
                    "[path=" + Path +
                    ", headers=" + Headers +
                    ", parameters=" + Parameters +
                    ", body.length=" + ((Body != null) ? Body.Length : 0) +
                    "]";
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
                method = "";
                path = "";
                query = null;
                headers = new Dictionary<string, List<string>>();
                body = null;
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
            public Builder Headers(Dictionary<string, string> headers)
            {
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                foreach(KeyValuePair<string,string> entry in headers)
                {
                    dict.Add(entry.Key, entry.Value.Split(new char[] { ',', ';' }).ToList());
                }
                this.headers = dict;
                return this;
            }

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

            //returns a new Nitric response.
            public NitricRequest Build()
            {
                //TODO: Add immutable dictionary if headers != null, otherwise empty dictionary
                var immutableHeaders = headers;
                this.method = (this.method == null) ? "" : this.method;
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
