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
using Nitric.Proto.Resources.v1;
using Nitric.Proto.Apis.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;
using ProtoApiResource = Nitric.Proto.Resources.v1.ApiResource;
using ProtoSecurityDefinition = Nitric.Proto.Resources.v1.ApiSecurityDefinitionResource;
using ProtoSecurityDefinitionJwt = Nitric.Proto.Resources.v1.ApiOpenIdConnectionDefinition;
using System.Net.Http;

namespace Nitric.Sdk.Resource
{
    public abstract class SecurityDefinition
    {
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

    public class ApiDetails
    {
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
            this.Middleware = middleware ?? Array.Empty<Middleware<HttpContext>>();
        }
    }


    public class ApiResource : BaseResource
    {
        internal readonly ApiOptions Opts;

        internal ApiResource(string name, ApiOptions options = null) : base(name, ResourceType.Api)
        {
            Opts = options ?? new ApiOptions();
        }

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void Get(string route, Func<HttpContext, HttpContext> handler) => Route(route).Get(handler);

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void Get(string route, params Middleware<HttpContext>[] middlewares) => Route(route).Get(middlewares);

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void Post(string route, Func<HttpContext, HttpContext> handler) => Route(route).Post(handler);

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void Post(string route, params Middleware<HttpContext>[] middlewares) => Route(route).Post(middlewares);

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void Put(string route, Func<HttpContext, HttpContext> handler) => Route(route).Put(handler);

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void Put(string route, params Middleware<HttpContext>[] middlewares) => Route(route).Put(middlewares);

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void Delete(string route, Func<HttpContext, HttpContext> handler) => Route(route).Delete(handler);

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void Delete(string route, params Middleware<HttpContext>[] middlewares) => Route(route).Delete(middlewares);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void Options(string route, Func<HttpContext, HttpContext> handler) => Route(route).Options(handler);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void Options(string route, params Middleware<HttpContext>[] middlewares) => Route(route).Options(middlewares);

        /// <summary>
        /// Create a new PATCH handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void Patch(string route, Func<HttpContext, HttpContext> handler) => Route(route).Patch(handler);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void Patch(string route, params Middleware<HttpContext>[] middlewares) => Route(route).Patch(middlewares);

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void All(string route, Func<HttpContext, HttpContext> handler) => Route(route).All(handler);

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="middlewares"></param>
        public void All(string route, params Middleware<HttpContext>[] middlewares) => Route(route).All(middlewares);

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
            var resource = new NitricResource { Name = this.Name, Type = ResourceType.Api };
            var apiResource = new ProtoApiResource();

            foreach (KeyValuePair<string, string[]> kv in this.Opts.Security)
            {
                var scopes = new ApiScopes();

                scopes.Scopes.Add(kv.Value);

                apiResource.Security.Add(kv.Key, scopes);
            }

            foreach (KeyValuePair<string, SecurityDefinition> kv in this.Opts.SecurityDefinitions)
            {
                var definition = new ProtoSecurityDefinition();

                if (kv.Value.Kind == "jwt")
                {
                    var jwtSecurityDefinition = kv.Value as JwtSecurityDefinition;
                    var secDef = new ProtoSecurityDefinitionJwt
                    {
                        Issuer = jwtSecurityDefinition.Issuer
                    };

                    secDef.Audiences.AddRange(jwtSecurityDefinition.Audiences);

                    definition.Oidc = secDef;
                }
            }

            var request = new ResourceDeclareRequest { Id = resource, Api = apiResource };
            BaseResource.client.Declare(request);

            return this;
        }
    }

    public class RouteOptions
    {
        // The middleware that is run on every route
        public Middleware<HttpContext>[] Middlewares { get; set; }

        // Security rules to apply to this specific route
        public Dictionary<string, string[]> Security { get; set; }

        public RouteOptions()
        {
            // Initialize Middlewares to an empty array to ensure it's never null
            this.Middlewares = Array.Empty<Middleware<HttpContext>>();
            this.Security = new Dictionary<string, string[]>();
        }
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

            var composedMiddleware = this.api.Opts.Middleware
                .Concat(opts.Middlewares)
                .ToArray();

            this.Opts = new RouteOptions
            {
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
        public void Get(Func<HttpContext, HttpContext> handler) => this.Method(new HttpMethod[] { HttpMethod.Get }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new GET middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Get(params Middleware<HttpContext>[] handlers) => Method(new HttpMethod[] { HttpMethod.Get }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public void Post(Func<HttpContext, HttpContext> handler) => Method(new HttpMethod[] { HttpMethod.Post }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new POST middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Post(params Middleware<HttpContext>[] handlers) => Method(new HttpMethod[] { HttpMethod.Post }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public void Put(Func<HttpContext, HttpContext> handler) => Method(new HttpMethod[] { HttpMethod.Put }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new PUT middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Put(params Middleware<HttpContext>[] handlers) => Method(new HttpMethod[] { HttpMethod.Post }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public void Delete(Func<HttpContext, HttpContext> handler) => Method(new HttpMethod[] { HttpMethod.Delete }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new DELETE middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Delete(params Middleware<HttpContext>[] handlers) => Method(new HttpMethod[] { HttpMethod.Delete }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public void Options(Func<HttpContext, HttpContext> handler) => Method(new HttpMethod[] { HttpMethod.Options }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new OPTIONS middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Options(params Middleware<HttpContext>[] handlers) => Method(new HttpMethod[] { HttpMethod.Options }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public void Patch(Func<HttpContext, HttpContext> handler) => Method(new HttpMethod[] { HttpMethod.Patch }, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new OPTIONS middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Patch(params Middleware<HttpContext>[] handlers) => Method(new HttpMethod[] { HttpMethod.Patch }, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="handler"></param>
        public void All(Func<HttpContext, HttpContext> handler) => Method((HttpMethod[])Enum.GetValues(typeof(HttpMethod)), ConcatMiddleware(handler));

        /// <summary>
        /// Create a new chain of middleware on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="handlers"></param>
        public void All(params Middleware<HttpContext>[] handlers) => Method((HttpMethod[])Enum.GetValues(typeof(HttpMethod)), ConcatMiddleware(handlers));

        internal void Method(HttpMethod[] methods, Func<HttpContext, HttpContext> handler)
        {
            var opts = new ApiWorkerOptions
            {
                SecurityDisabled = true,
            };

            if (this.Opts.Security.Count > 0)
            {
                var security = this.Opts.Security.ToDictionary((kv) => kv.Key, kv =>
                {
                    var scopes = new ApiWorkerScopes();
                    scopes.Scopes.Add(kv.Value);

                    return scopes;
                });

                opts.Security.Add(security);
                opts.SecurityDisabled = false;
            }

            var registrationRequest = new RegistrationRequest
            {
                Api = this.api.Name,
                Options = opts,
                Path = this.Path,
            };

            registrationRequest.Methods.AddRange(methods.Select((method) => method.Method).ToHashSet());


            var apiWorker = new ApiWorker(registrationRequest, handler);

            Nitric.RegisterWorker(apiWorker);
        }

        internal void Method(HttpMethod[] methods, Middleware<HttpContext>[] middleware)
        {
            var opts = new ApiWorkerOptions
            {
                SecurityDisabled = true,
            };

            if (this.Opts.Security.Count > 0)
            {
                var security = this.Opts.Security.ToDictionary((kv) => kv.Key, kv =>
                {
                    var scopes = new ApiWorkerScopes();
                    scopes.Scopes.Add(kv.Value);

                    return scopes;
                });

                opts.Security.Add(security);
                opts.SecurityDisabled = false;
            }

            var registrationRequest = new RegistrationRequest
            {
                Api = this.api.Name,
                Options = opts,
                Path = this.Path,
            };

            registrationRequest.Methods.AddRange(methods.Select((method) => method.Method).ToHashSet());


            var apiWorker = new ApiWorker(registrationRequest, middleware);

            Nitric.RegisterWorker(apiWorker);
        }
    }
}