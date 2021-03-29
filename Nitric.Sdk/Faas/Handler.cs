using System;
using System.Collections.Generic;
namespace Nitric.Api.Faas
{
    public class Handler
    {
        private string hostname = "127.0.0.1";
        private int port = 42069;
        Dictionary<string, INitricFunction> pathFunctions = new Dictionary<string, INitricFunction>();
        public  Func<NitricRequest> func;
        public Handler(Func<NitricRequest> func)
        {
            this.func = func;
        }
        public void CallHandler(string path, string[] args)
        {

        }
    }
}
