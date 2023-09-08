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
using Google.Api;
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
            Dictionary<string, SecurityDefinition> securityDefinitions = null,
            Dictionary<string, string[]> security = null,
            string basePath = "",
            Middleware<HttpContext>[] middleware = null
        )
        {
            this.SecurityDefinitions = securityDefinitions ?? new Dictionary<string, SecurityDefinition>();
            this.Security = security ?? new Dictionary<string, string[]>();
            this.BasePath = basePath;
            this.Middleware = middleware ?? new Middleware<HttpContext>[] { };
        }
    }


    public class ApiResource : BaseResource
    {
        internal readonly ApiOptions Opts;

        internal ApiResource(string name, ApiOptions options = null) : base(name, ResourceType.Api)
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
            return new ApiRoute(this, this.Opts.BasePath + path, new RouteOptions());
        }

        /// <summary>
        /// Create a new route on a specified path.
        /// </summary>
        /// <returns>An ApiRoute that handlers can be added to.</returns>
        /// <param name="path"></param>
        /// <param name="middleware"></param>
        public ApiRoute Route(string path, RouteOptions options)
        {
            return new ApiRoute(this, this.Opts.BasePath + path, options);
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
                    var jwtSecurityDefinition = kv.Value as JwtSecurityDefinition;
                    var secDef = new ApiSecurityDefinitionJwt
                    {
                        Issuer = jwtSecurityDefinition.Issuer
                    };

                    secDef.Audiences.AddRange(jwtSecurityDefinition.Audiences);

                    definition.Jwt = secDef;
                }

                apiResource.SecurityDefinitions.Add(kv.Key, definition);
            }

            var request = new ResourceDeclareRequest { Resource = resource, Api = apiResource };
            BaseResource.client.Declare(request);

            return this;
        }

        /// <summary>
        /// Retrieve details about the deployed API at runtime. These details include:
        /// - ID: the identifier for the resource.
        /// - Provider: the cloud provider that this API is deployed to.
        /// - Service: the cloud service that is running this API (i.e. AWS API Gateway).
        /// - URL: the url of the deployed API.
        /// </summary>
        /// <returns>The details of the API</returns>
        public ApiDetails Details() {
            var resource = new NitricResource { Name = this.name, Type = ResourceType.Api };

            var request = new ResourceDetailsRequest { Resource = resource };
            var response = client.Details(request);

            return new ApiDetails
            {
                ID = response.Id,
                Provider = response.Provider,
                Service = response.Service,
                URL = response.Api.Url,
            };
        }
    }

    public class RouteOptions
    {
        // The middleware that is run on every route
        public Middleware<HttpContext>[] Middlewares { get; set; }

        // Security rules to apply to this specific route
        public Dictionary<string, string[]> Security { get; set; }
    }

    public class ApiRoute
    {
        // The api that this route is on
        private readonly ApiResource api;

        // The path that this route's handlers respond to
        public readonly string Path;

        // Options for the API route, including middleware and security
        public readonly RouteOptions Opts;

        internal ApiRoute(ApiResource api, string path, RouteOptions opts)
        {
            this.api = api;
            this.Path = path;

            var composedMiddleware = this.api.Opts.Middleware.Concat(opts.Middlewares).ToArray();
            this.Opts = new RouteOptions {
                Middlewares = composedMiddleware,
                Security = opts.Security
            };
        }

        private Middleware<HttpContext>[] ConcatMiddleware(Func<HttpContext, HttpContext> handler)
        {
            HttpContext ComposedMiddleware(HttpContext context, Func<HttpContext, HttpContext> next)
            {
                context = handler(context);
                return next(context);
            };
            return this.Opts.Middlewares.Append(ComposedMiddleware).ToArray();
        }

        private Middleware<HttpContext>[] ConcatMiddleware(Middleware<HttpContext>[] middlewares)
        {
            return this.Opts.Middlewares.Concat(middlewares).ToArray();
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
