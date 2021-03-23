using System;
using System.Net;

namespace Nitric.Api.Faas
{
    public class Response
    {
        public string Body { get; set; }
        public int Status { get; set; }
        public Object Headers { get; set; }
        public Response(string body = "", int status = (int)HttpStatusCode.OK, Object headers = null)
        {
            if (headers == null)
            {
                headers = new Object();
            }
            Body = body;
            Status = status;
            Headers = headers;
        }
    }
}
