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

namespace Nitric.Faas
{
    public class HttpResponseContext : ResponseContext
    {
        private int status = 200;
        private Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

        public int GetStatus()
        {
            return this.status;
        }
        public Dictionary<string, List<string>> GetHeaders()
        {
            return this.headers;
        }
        public HttpResponseContext SetStatus(int status)
        {
            this.status = status;
            return this;
        }
        public HttpResponseContext AddHeader(string key, string value)
        {
            var arr = this.headers.GetValueOrDefault(key, new List<string>());
            arr.Add(value);

            this.headers.Add(key, arr);

            return this;
        }
        public HttpResponseContext SetHeaders(Dictionary<string, List<string>> headers)
        {
            this.headers = headers;
            return this;
        }
    }
}
