using System;
using System.Net;
using System.IO;
using Nitric.Api.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
namespace Nitric.Api.Faas
{
    /**
     * <summary>Contains all the helper functions for Faas</summary>
     */
    public class Faas
    {
        string hostname = "127.0.0.1";
        public string HostName
        {
            get => hostname;
            set
            {
                hostname = value;
            }
        }
        int port = 8080;
        public int Port
        {
            get => port;
            set
            {
                port = value;
            }
        }
        HttpListener listener;
        public HttpListener Listener
        {
            get => listener;
            set { listener = value; }
        }
        INitricFunction function;

        public void Start(INitricFunction function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("null function parameter");
            }
            if (listener != null)
            {
                throw new ArgumentException("listener has already started");
            }
            this.function = function;
            long time = DateTime.Now.Millisecond;

            var childAddress = Environment.GetEnvironmentVariable("CHILD_ADDRESS");
            if (!string.IsNullOrEmpty(childAddress))
            {
                hostname = childAddress;
            }
            Console.WriteLine(string.Format("Starting Faas Function {0} at:\nhttp://{1}:{2}/*/", function.GetType().Name, hostname, port));
            this.listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://{0}:{1}/",hostname,port));
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
            while (true)
            {
                // Will wait here until we hear from a connection
                Task<HttpListenerContext> taskContext = Task.Run(async () => await listener.GetContextAsync());
                OnContext(taskContext.Result);
            }
        }
        private void OnContext(HttpListenerContext ctx)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " Handling request");
            ctx.Response.ContentType = "text/plain";

            //Reads the request input stream
            var requestStreamReader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
            var requestBody = requestStreamReader.ReadToEnd();
            requestStreamReader.Close();
            //Builds a new NitricRequest based on the HttpContextRequest
            NitricRequest request = new NitricRequest.Builder()
                .Path(ctx.Request.RawUrl)
                .Headers(Util.NameValueCollecToDict(ctx.Request.Headers))
                .Query(ctx.Request.QueryString.ToString())
                .Method(ctx.Request.HttpMethod)
                .Body(Encoding.UTF8.GetBytes(requestBody.ToString().ToCharArray()))
                .Build();
            //Calls the user's NitricFunction handler, parsing in the request and returns the response
            var functionResponse = function.Handle(request);
            //Converts the NitricResponse object into a HttpResponse 
            foreach(KeyValuePair<string, List<string>> entry in functionResponse.Headers)
            {
                ctx.Response.AddHeader(entry.Key, entry.Value.ToString());
            }            
            ctx.Response.StatusCode = (int)functionResponse.Status;
            ctx.Response.OutputStream.Write(functionResponse.Body, 0, functionResponse.Body.Length);
            ctx.Response.Close();
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " completed");
        }
    }
}
