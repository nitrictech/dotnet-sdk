using System.Collections.Generic;

namespace Nitric.Faas
{
    public class HttpResponseContext : ResponseContext
    {
        private int status = 200;
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        public int GetStatus()
        {
            return this.status;
        }
        public Dictionary<string, string> GetHeaders()
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
            this.headers.Add(key, value);
            return this;
        }
        public HttpResponseContext SetHeaders(Dictionary<string, string> headers)
        {
            this.headers = headers;
            return this;
        }
    }
}
