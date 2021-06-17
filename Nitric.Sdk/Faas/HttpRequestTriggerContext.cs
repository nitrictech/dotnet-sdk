using System.Collections.Generic;

namespace Nitric.Faas
{
    public class HttpRequestTriggerContext : TriggerContext
    {
        public string Method { get; private set; }
        public string Path { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public Dictionary<string, string> QueryParams { get; private set; }

        public HttpRequestTriggerContext(
            string method,
            string path,
            Dictionary<string, string> headers,
            Dictionary<string, string> queryParams
        )
        {
            this.Method = method;
            this.Path = path;
            this.Headers = headers;
            this.QueryParams = queryParams;
        }
    }
}
