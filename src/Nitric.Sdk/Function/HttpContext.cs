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
using Nitric.Proto.Faas.v1;
using Nitric.Sdk.Common;
using TriggerRequestProto = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Sdk.Function
{
    /// <summary>
    /// Represents a HTTP request.
    /// </summary>
    public class HttpRequest : AbstractRequest
    {
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

        public byte[] Data { get; private set; }

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
            Dictionary<string, IEnumerable<string>> headers) : base(data)
        {
            this.Path = path;
            this.PathParams = pathParams;
            this.QueryParams = queryParams;
            this.Headers = headers;
        }
    }

    /// <summary>
    /// The response to an HTTP trigger.
    /// </summary>
    public class HttpResponse
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
        public HttpContext(HttpRequest req, HttpResponse res) : base(req, res)
        {
        }

        /// <summary>
        /// Construct a C# SDK HttpContext object from the gRPC wire TriggerRequest
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static HttpContext FromGrpcTriggerRequest(TriggerRequestProto trigger)
        {
            var headers = new Dictionary<string, IEnumerable<string>>();
            foreach (var kv in trigger.Http.Headers)
            {
                var values = headers.GetValueOrDefault(kv.Key, new List<string>());
                headers.Add(kv.Key, values.Concat(kv.Value.Value));
            }

            var queryParams = new Dictionary<string, IEnumerable<string>>();
            foreach (var kv in trigger.Http.QueryParams)
            {
                var values = queryParams.GetValueOrDefault(kv.Key, new List<string>());
                queryParams.Add(kv.Key, values.Concat(kv.Value.Value));
            }

            var pathParams = trigger.Http.PathParams.ToDictionary(kv => kv.Key, kv => kv.Value);

            return new HttpContext(
                new HttpRequest(trigger.Data.ToByteArray(), trigger.Http.Method, trigger.Http.Path, pathParams,
                    queryParams, headers),
                new HttpResponse()
            );
        }

        /// <summary>
        /// Create a gRPC trigger response from this context.
        /// </summary>
        /// <returns></returns>
        public override TriggerResponse ToGrpcTriggerContext()
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
            var triggerResponse = new TriggerResponse
                { Http = new HttpResponseContext { Status = this.Res.Status } };
            triggerResponse.Http.Headers.Add(responseHeaders);
            triggerResponse.Data = ByteString.CopyFrom(this.Res.Body);

            return triggerResponse;
        }
    }
}
