using System;
namespace Nitric.Api.Faas
{
    public class RequestParameter
    {
        public Object Path { get; set; }
        public Object Query { get; set; }
        public RequestParameter(Object path = null, Object query = null)
        {
            if (path == null)
            {
                path = new Object();
            }
            if (query == null)
            {
                query = new Object();
            }
            Path = path;
            Query = query;
        }
    }
}
