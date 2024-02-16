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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Nitric.Proto.Apis.v1;

namespace Nitric.Sdk.Service
{
    /// <summary>
    /// Represents a HTTP request.
    /// </summary>
    public class HttpRequest : TriggerRequest
    {
        /// <summary>
        /// The raw bytes.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// The HTTP Method that generated this request. E.g. GET, POST, PUT, DELETE.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// The HTTP path requested.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The auto-extracted path parameter values.
        ///
        /// E.g. for a path "/customers/:id" PathParams would contain an "id" key with the provided id value.
        /// </summary>
        public Dictionary<string, string> PathParams { get; private set; }

        /// <summary>
        /// Contains any provided query parameters.
        /// </summary>
        public Dictionary<string, IEnumerable<string>> QueryParams { get; private set; }

        /// <summary>
        /// Contains any HTTP headers provided with the request.
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Headers { get; private set; }

        /// <summary>
        /// Convert the payload of the request to a string, assuming UTF-8 encoding.
        /// </summary>
        /// <returns></returns>
        public string Text => Encoding.UTF8.GetString(this.data, 0, this.data.Length);

        /// <summary>
        /// Get the HTTP body data encoded from json.
        ///
        /// The object is converted from a JSON string to a type T.
        /// </summary>
        public T Json<T>() {
            return JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(this.data));
        }

        /// <summary>
        /// Constructs a new HTTP Request object.
        /// </summary>
        /// <param name="data">HTTP request body data</param>
        /// <param name="method">the HTTP method</param>
        /// <param name="path">the requested path</param>
        /// <param name="pathParams">any auto-extracted path parameters</param>
        /// <param name="queryParams">any provided HTTP query parameters</param>
        /// <param name="headers">any provided HTTP headers</param>
        public HttpRequest(byte[] data, string method, string path, Dictionary<string, string> pathParams,
            Dictionary<string, IEnumerable<string>> queryParams,
            Dictionary<string, IEnumerable<string>> headers) : base()
        {
            this.data = data;
            this.Path = path;
            this.PathParams = pathParams;
            this.QueryParams = queryParams;
            this.Headers = headers;
        }
    }

    /// <summary>
    /// The response to an HTTP trigger.
    /// </summary>
    public class HttpResponse : TriggerResponse
    {
        /// <summary>
        /// The HTTP status code of the response.
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// The HTTP body data to be returned.ß
        /// </summary>
        public byte[] Body { get; set; } = {};

        /// <summary>
        /// The HTTP header to be included in the response.
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Headers { get; set; } = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Set the HTTP body data from an object.
        ///
        /// The object is converted to a JSON string with UTF-8 encoding and the Content-Type header is set to application/json.
        /// </summary>
        public void Json(object obj)
        {
            this.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            this.Headers["Content-Type"] = new List<string> { "application/json" };
        }

        /// <summary>
        /// Set the HTTP body data from a string.
        ///
        /// The object is converted to bytes assuming UTF-8 encoding and the Content-Type header is set to text/plain.
        /// </summary>
        public void Text(string text)
        {
            this.Body = Encoding.UTF8.GetBytes(text);
            this.Headers["Content-Type"] = new List<string> { "text/plain" };
        }
    }

    /// <summary>
    /// An HTTP context containing a request and response object.
    /// </summary>
    public class HttpContext : TriggerContext<HttpRequest, HttpResponse>
    {
        /// <summary>
        /// Construct a new HTTP context object.
        /// </summary>
        /// <param name="req">The context's request</param>
        /// <param name="res">The context's response</param>
        public HttpContext(string id, HttpRequest req, HttpResponse res) : base(id, req, res)
        {
        }

        /// <summary>
        /// Construct a C# SDK HttpContext object from the gRPC wire TriggerRequest
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        internal static HttpContext FromRequest(ServerMessage trigger)
        {
            var headers = new Dictionary<string, IEnumerable<string>>();
            foreach (var kv in trigger.HttpRequest.Headers)
            {
                var values = headers.GetValueOrDefault(kv.Key, new List<string>());
                headers.Add(kv.Key, values.Concat(kv.Value.Value));
            }

            var queryParams = new Dictionary<string, IEnumerable<string>>();
            foreach (var kv in trigger.HttpRequest.QueryParams)
            {
                var values = queryParams.GetValueOrDefault(kv.Key, new List<string>());
                queryParams.Add(kv.Key, values.Concat(kv.Value.Value));
            }

            var pathParams = trigger.HttpRequest.PathParams.ToDictionary(kv => kv.Key, kv => kv.Value);

            return new HttpContext(
                trigger.Id,
                new HttpRequest(trigger.HttpRequest.Body.ToByteArray(), trigger.HttpRequest.Method, trigger.HttpRequest.Path, pathParams,
                    queryParams, headers),
                new HttpResponse()
            );
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        internal ClientMessage ToResponse()
        {
            var responseHeaders = new MapField<string, HeaderValue>
            {
                this.Res.Headers.ToDictionary(h => h.Key, h =>
                {
                    var hv = new HeaderValue();
                    hv.Value.Add(h.Value);
                    return hv;
                })
            };

            var triggerResponse = new ClientMessage {
                Id = Id,
                HttpResponse = new Proto.Apis.v1.HttpResponse
                {
                    Status = this.Res.Status,
                    Body = ByteString.CopyFrom(this.Res.Body),
                }
            };

            triggerResponse.HttpResponse.Headers.Add(responseHeaders);

            return triggerResponse;
        }
    }
}
