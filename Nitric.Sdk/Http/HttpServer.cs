using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
namespace Nitric.Api.Http
{
    public class HttpServer
    {
        private static readonly string DefaultHostName = "127.0.0.1";
        private static readonly int DefaultPort = 8080;

        private string hostname = DefaultHostName;
        public string Hostname
        {
            get => hostname;
            set
            {
                hostname = value;
            }
        }
        private int port = DefaultPort;
        public int Port
        {
            get => port;
            set
            {
                port = value;
            }
        }
        public HttpListener Listener { get; private set; }
        readonly Dictionary<string, IHttpHandler> pathFunctions = new Dictionary<string, IHttpHandler>();

        public HttpServer Register(string path, IHttpHandler function)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (function == null)
            {
                throw new ArgumentNullException("function");
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
            if (Listener != null)
            {
                throw new MethodAccessException("listener has already started");
            }
            long time = DateTime.Now.Millisecond;

            var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
            if (!string.IsNullOrEmpty(childAddress))
            {
                Hostname = childAddress;
            }
            foreach (string s in pathFunctions.Keys)
            {
                var function = pathFunctions[s];
                Listener.Prefixes.Add(string.Format("http://{0}:{1}/{2}/", Hostname, Port, s));
            }


            Listener.Start();

            var builder = new StringBuilder().Append(GetType().Name);
            if (Hostname == DefaultHostName)
            {
                builder.Append(" listening on port ").Append(Port);
            }
            else
            {
                builder.Append(" listening on ").Append(Hostname).Append(":").Append(Port);
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
