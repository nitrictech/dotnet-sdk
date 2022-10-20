using System;
using System.Linq;
using System.Text;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Function;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;

namespace Nitric.Sdk.Resource
{
    public class ApiResource : BaseResource
    {
        internal ApiResource(string name) : base(name)
        {
        }

        private ApiResource Method(string route, Func<HttpContext, HttpContext> handler, params HttpMethod[] methods)
        {
            var apiWorker = new Faas
            {
                HttpHandler = handler,
                Options = new ApiWorkerOptions{ Api = this.name, Route = route, Methods = methods.ToHashSet() },
            };
            Nitric.RegisterWorker(apiWorker);
            return this;
        }

        public ApiResource Get(string route, Func<HttpContext, HttpContext> handler) => Method(route, handler, HttpMethod.GET);
        public ApiResource Post(string route, Func<HttpContext, HttpContext> handler) => Method(route, handler, HttpMethod.POST);
        public ApiResource Put(string route, Func<HttpContext, HttpContext> handler) => Method(route, handler, HttpMethod.PUT);
        public ApiResource Delete(string route, Func<HttpContext, HttpContext> handler) => Method(route, handler, HttpMethod.DELETE);
        public ApiResource Options(string route, Func<HttpContext, HttpContext> handler) => Method(route, handler, HttpMethod.OPTIONS);
        public ApiResource All(string route, Func<HttpContext, HttpContext> handler) => Method(route, handler);

        internal override BaseResource Register()
        {
            // TODO: Implement me.
            throw new NotImplementedException();
        }
    }
}
