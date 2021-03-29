using System;
using System.Collections.Generic;
using Nitric.Api.Common;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
namespace Nitric.Api.Faas
{
    /**
     * <summary>Contains all the helper functions for Faas</summary>
     */
    public class Faas
    {
        string hostname = "127.0.0.1";
        int port = 8080;
        HttpListener listener;

        public Faas HostName(string hostname)
        {
            this.hostname = hostname;
            return this;
        }
        public Faas Port(int port)
        {
            this.port = port;
            return this;
        }

        public void Start(INitricFunction function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("null function parameter");
            }
            long time = DateTime.Now.Millisecond;

            var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
            if (!string.IsNullOrEmpty(childAddress))
            {
                hostname = childAddress;
            }

            try
            {
                this.listener = new HttpListener();
                listener.Prefixes.Add("**");
                listener.Prefixes.Add(string.Format("{0}:{1}",hostname,port));
                listener.Start();
                var builder = new StringBuilder()
                .Append(GetType().ToString())
                .Append(" listening on port ")
                .Append(port)
                .Append(" with function: ");

                if (!string.IsNullOrEmpty(function.GetType().Name))
                {
                    builder.Append(function.GetType().Name);
                }
                else
                {
                    builder.Append(function.GetType().FullName);
                }
                Console.WriteLine(builder);

            } catch(IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
