// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nitric.Proto.Resource.v1;
using Nitric.Sdk.Function;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;
using NitricResource = Nitric.Proto.Resource.v1.Resource;
using ProtoApiResource = Nitric.Proto.Resource.v1.ApiResource;
using ProtoSecurityDefinition = Nitric.Proto.Resource.v1.ApiSecurityDefinition;
using ProtoSecurityDefinitionJwt = Nitric.Proto.Resource.v1.ApiSecurityDefinitionJwt;


namespace Nitric.Sdk.Resource
{
    public abstract class SecurityDefinition {
        internal string Kind { get; private set; }

        internal SecurityDefinition(string kind)
        {
            this.Kind = kind;
        }
    }

    public class JwtSecurityDefinition : SecurityDefinition
    {
        internal string Issuer { get; private set; }
        internal string[] Audiences { get; private set; }

        internal JwtSecurityDefinition(string Issuer, string[] Audiences) : base("jwt")
        {
            this.Issuer = Issuer;
            this.Audiences = Audiences;
        }                
    }   

    public class ApiDetails {
        internal string ID { get; set; }
        internal string Provider { get; set; }
        internal string Service { get; set; }
        internal string URL { get; set; }
    }


    public class ApiOptions
    {
        public Dictionary<string, SecurityDefinition> SecurityDefinitions { get; private set; }
        public Dictionary<string, string[]> Security { get; private set; }
        public string BasePath { get; private set; }
        public Middleware<HttpContext>[] Middleware { get; private set; }

        public ApiOptions(
            Dictionary<string, SecurityDefinition> SecurityDefinitions = null,
            Dictionary<string, string[]> Security = null,
            string BasePath = "",
            Middleware<HttpContext>[] Middleware = null
        )
        {
            this.SecurityDefinitions = SecurityDefinitions ?? new Dictionary<string, SecurityDefinition>();
            this.Security = Security ?? new Dictionary<string, string[]>();
            this.BasePath = BasePath;
            this.Middleware = Middleware ?? new Middleware<HttpContext>[] { };
        }
    }


    public class ApiResource : BaseResource
    {
        ApiOptions Opts;

        internal ApiResource(string name, ApiOptions options = null) : base(name)
        {
            this.Opts = options ?? new ApiOptions();
        }

        internal ApiResource Method(string route, HttpMethod[] methods, Func<HttpContext, HttpContext> handler)
        {
            var opts = new MethodOptions
            {
                Security = this.Opts.Security,
                SecurityDefs = this.Opts.SecurityDefinitions
            };

            var faas = new Faas(new ApiWorkerOptions
            {
                Api = this.name,
                Route = this.Opts.BasePath + route,
                Methods = methods.ToHashSet(),
                Options = opts
            });

            faas.Http(this.Opts.Middleware);

            faas.Http(handler);

            Nitric.RegisterWorker(faas);
            return this;
        }

        internal ApiResource Method(string route, HttpMethod[] methods, Middleware<HttpContext>[] middleware)
        {
            var opts = new MethodOptions
            {
                Security = this.Opts.Security,
                SecurityDefs = this.Opts.SecurityDefinitions
            };

            var faas = new Faas(new ApiWorkerOptions
            {
                Api = this.name,
                Route = this.Opts.BasePath + route,
                Methods = methods.ToHashSet(),
                Options = opts
            });

            var combinedMiddleware = this.Opts.Middleware.Concat(middleware).ToArray();

            faas.Http(combinedMiddleware);

            Nitric.RegisterWorker(faas);
            return this;
        }

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public ApiResource Get(string route, Func<HttpContext, HttpContext> handler) => Method(route, new HttpMethod[] { HttpMethod.GET }, handler);

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handlers"></param>
        public ApiResource Get(string route, params Middleware<HttpContext>[] handlers) => Method(route, new HttpMethod[] { HttpMethod.GET }, handlers);

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>        
        public ApiResource Post(string route, Func<HttpContext, HttpContext> handler) => Method(route, new HttpMethod[] { HttpMethod.POST }, handler);

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handlers"></param>
        public ApiResource Post(string route, params Middleware<HttpContext>[] handlers) => Method(route, new HttpMethod[] { HttpMethod.POST }, handlers);

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public ApiResource Put(string route, Func<HttpContext, HttpContext> handler) => Method(route, new HttpMethod[] { HttpMethod.PUT }, handler);

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handlers"></param>
        public ApiResource Put(string route, params Middleware<HttpContext>[] handlers) => Method(route, new HttpMethod[] { HttpMethod.PUT }, handlers);

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public ApiResource Delete(string route, Func<HttpContext, HttpContext> handler) => Method(route, new HttpMethod[] { HttpMethod.DELETE }, handler);

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handlers"></param>
        public ApiResource Delete(string route, params Middleware<HttpContext>[] handlers) => Method(route, new HttpMethod[] { HttpMethod.DELETE }, handlers);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public ApiResource Options(string route, Func<HttpContext, HttpContext> handler) => Method(route, new HttpMethod[] { HttpMethod.OPTIONS }, handler);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handlers"></param>
        public ApiResource Options(string route, params Middleware<HttpContext>[] handlers) => Method(route, new HttpMethod[] { HttpMethod.OPTIONS }, handlers);

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public ApiResource All(string route, Func<HttpContext, HttpContext> handler) => Method(route, (HttpMethod[])Enum.GetValues(typeof(HttpMethod)), handler);

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handlers"></param>
        public ApiResource All(string route, params Middleware<HttpContext>[] handlers) => Method(route, (HttpMethod[])Enum.GetValues(typeof(HttpMethod)), handlers);

        /// <summary>
        /// Create a new route on a specified path.
        /// </summary>
        /// <returns>An ApiRoute that handlers can be added to.</returns>
        /// <param name="path"></param>
        public ApiRoute Route(string path)
        {
            return new ApiRoute(this, this.Opts.BasePath + path);
        }

        /// <summary>
        /// Create a new route on a specified path.
        /// </summary>
        /// <returns>An ApiRoute that handlers can be added to.</returns>
        /// <param name="path"></param>
        /// <param name="middleware"></param>
        public ApiRoute Route(string path, params Middleware<HttpContext>[] middleware)
        {
            return new ApiRoute(this, this.Opts.BasePath + path, middleware);
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Api };
            var apiResource = new ProtoApiResource();

            foreach (KeyValuePair<string, string[]> kv in this.Opts.Security)
            {
                var scopes = new ApiScopes();

                scopes.Scopes.Add(kv.Value);

                apiResource.Security.Add(kv.Key, scopes);
            }

            foreach (KeyValuePair<string, SecurityDefinition> kv in this.Opts.SecurityDefinitions) {
                var definition = new ProtoSecurityDefinition();

                if (kv.Value.Kind == "jwt")
                {
                    var secDef = new ApiSecurityDefinitionJwt();

                    secDef.Issuer = (kv.Value as JwtSecurityDefinition).Issuer;
                    secDef.Audiences.AddRange((kv.Value as JwtSecurityDefinition).Audiences);

                    definition.Jwt = secDef;
                }

                apiResource.SecurityDefinitions.Add(kv.Key, definition);
            }            

            var request = new ResourceDeclareRequest { Resource = resource, Api = apiResource };
            BaseResource.client.Declare(request);

            return this;
        }

        internal ApiDetails Details() {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Api };

            var request = new ResourceDetailsRequest { Resource = resource };
            var response = BaseResource.client.Details(request);

            return new ApiDetails
            {
                ID = response.Id,
                Provider = response.Provider,
                Service = response.Service,
                URL = response.Api.Url,
            };
        }

        public string URL()
        {
            return this.Details().URL;
        }
    }

    public class ApiRoute
    {
        // The api that this route is on
        private readonly ApiResource api;

        // The path that this route's handlers respond to
        public readonly string Path;

        // The middleware that is run on every route
        private readonly Middleware<HttpContext>[] middleware;

        internal ApiRoute(ApiResource api, string path, params Middleware<HttpContext>[] middleware)
        {
            this.api = api;
            this.Path = path;
            this.middleware = middleware;
        }

        internal Middleware<HttpContext>[] ConcatMiddleware(Func<HttpContext, HttpContext> handler)
        {
            Middleware<HttpContext> middleware = (context, next) =>
            {
                context = handler(context);
                return next(context);
            };
            return this.middleware.Append(middleware).ToArray();
        }

        internal Middleware<HttpContext>[] ConcatMiddleware(Middleware<HttpContext>[] middleware)
        {
            return this.middleware.Concat(middleware).ToArray();
        }

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public ApiResource Get(Func<HttpContext, HttpContext> handler) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.GET }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new GET middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public ApiResource Get(params Middleware<HttpContext>[] handlers) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.GET }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public ApiResource Post(Func<HttpContext, HttpContext> handler) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.POST }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new POST middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public ApiResource Post(params Middleware<HttpContext>[] handlers) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.POST}, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public ApiResource Put(Func<HttpContext, HttpContext> handler) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.PUT}, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new PUT middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public ApiResource Put(params Middleware<HttpContext>[] handlers) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.POST }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public ApiResource Delete(Func<HttpContext, HttpContext> handler) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.DELETE }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new DELETE middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public ApiResource Delete(params Middleware<HttpContext>[] handlers) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.DELETE }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public ApiResource Options(Func<HttpContext, HttpContext> handler) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.OPTIONS }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new OPTIONS middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public ApiResource Options(params Middleware<HttpContext>[] handlers) => this.api.Method(this.Path, new HttpMethod[] { HttpMethod.OPTIONS }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="handler"></param>
        public ApiResource All(Func<HttpContext, HttpContext> handler) => this.api.Method(this.Path, (HttpMethod[])Enum.GetValues(typeof(HttpMethod)), ConcatMiddleware(handler));

        /// <summary>
        /// Create a new chain of middleware on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="handlers"></param>
        public ApiResource All(params Middleware<HttpContext>[] handlers) => this.api.Method(this.Path, (HttpMethod[])Enum.GetValues(typeof(HttpMethod)), ConcatMiddleware(handlers));
    }
}
