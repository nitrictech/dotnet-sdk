using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
namespace Nitric.Api.Http
{
    public class HttpServer
    {
        static readonly string DefaultHostname = "127.0.0.1";

        string hostname = DefaultHostname;
        int port = 8080;
        readonly Dictionary<string, IHttpHandler> pathFunctions = new Dictionary<string, IHttpHandler>();
        HttpListener httpListener = new HttpListener();

        public HttpServer Hostname(string hostname)
        {
            this.hostname = hostname;
            return this;
        }
        public HttpServer Port(int port)
        {
            this.port = port;
            return this;
        }
        public HttpServer Register(string path, IHttpHandler function)
        {
            if (path == null)
            {
                throw new ArgumentNullException("Null path parameter");
            }
            if (function == null)
            {
                throw new ArgumentNullException("Null function parameter");
            }
            IHttpHandler checkHandler = null;
            if (pathFunctions.ContainsKey(path))
            {
                checkHandler = pathFunctions[path];
                pathFunctions.Add(path, function);
                return this;
            }
            var msg = checkHandler.GetType().Name + " already registered for path: " + path;
            throw new ArgumentException(msg);
        }
        public void Start(IHttpHandler function)
        {
            Register("/", function);
            Start();
        }
        public void Start()
        {
            if (httpListener != null)
            {
                throw new MethodAccessException("listener has already started");
            }
            long time = DateTime.Now.Millisecond;

            var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
            if (!string.IsNullOrEmpty(childAddress))
            {
                hostname = childAddress;
            }
            foreach (string s in pathFunctions.Keys)
            {
                var function = pathFunctions[s];
                httpListener.Prefixes.Add(string.Format("{0}{1}", s, pathFunctions[s].ToString()));
            }


            httpListener.Start();

            var builder = new StringBuilder().Append(GetType().Name);
            if (DefaultHostname == hostname)
            {
                builder.Append(" listening on port ").Append(port);
            }
            else
            {
                builder.Append(" listening on ").Append(hostname).Append(":").Append(port);
            }

            if (pathFunctions.Count == 0)
            {
                builder.Append(" - WARN No functions registered");
            }
            else if (pathFunctions.Count == 1)
            {
                builder.Append(" with function:");
            }
            else if (pathFunctions.Count > 1)
            {
                builder.Append(" with functions:");
            }
            Console.WriteLine(builder);


            if (pathFunctions.Count > 0)
            {
                foreach (string path in pathFunctions.Keys)
                {
                    var functionClass = pathFunctions[path].GetType();
                    var functionClassName = !string.IsNullOrEmpty(functionClass.Name)
                            ? functionClass.Name : functionClass.FullName;

                    Console.WriteLine("{0}\t-> {1}\n", path, functionClassName);
                }
            }
        }
    }
}
