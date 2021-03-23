using System;
using System.Collections.Generic;
namespace Nitric.Api.Faas
{
    public class Request
    {
        public Context Context { get; set; }
        public Request(Dictionary<string, string> headers, byte[] payload, string path="")
        {
            Dictionary<string, string> contextProps = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> entry in headers)
            {
                if (entry.Key.ToLower().StartsWith("x-nitric"))
                {
                    contextProps.Add(Context.CleanHeader(entry.Key),entry.Value);
                }
            }
            
        }
    }
}
