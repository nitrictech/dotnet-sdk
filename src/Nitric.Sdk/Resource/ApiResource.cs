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
using Nitric.Proto.Resources.v1;
using Nitric.Proto.Apis.v1;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;
using ProtoApiResource = Nitric.Proto.Resources.v1.ApiResource;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nitric.Sdk.Resource
{
    public class ApiDetails
    {
        internal string ID { get; set; }
        internal string Provider { get; set; }
        internal string Service { get; set; }
        internal string URL { get; set; }
    }

    public class ApiOptions
    {
        public OidcOptions[] Security { get; private set; }
        public string BasePath { get; private set; }
        public Middleware<HttpContext>[] Middleware { get; private set; }

        public ApiOptions(
            OidcOptions[] security = null,
            string basePath = "",
            Middleware<HttpContext>[] middleware = null
        )
        {
            this.Security = security ?? Array.Empty<OidcOptions>();
            this.BasePath = basePath;
            this.Middleware = middleware ?? Array.Empty<Middleware<HttpContext>>();
        }
    }

    public class ApiResource : BaseResource
    {
        public readonly ApiOptions Opts;

        internal ApiResource(string name, ApiOptions options = null) : base(name, ResourceType.Api)
        {
            Opts = options ?? new ApiOptions();
        }

        internal void AttachOidc(OidcOptions opts)
        {
            var oidcName = string.Format("{0}-{1}", opts.Name, this.Name);
            Nitric.Register(oidcName, _ => new OidcResource(oidcName, this.Name, opts));
        }

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handler">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Get(string route, Func<HttpContext, HttpContext> handler, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Get(handler);

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handlers">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Get(string route, Middleware<HttpContext>[] handlers, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Get(handlers);

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handler">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Post(string route, Func<HttpContext, HttpContext> handler, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Post(handler);

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handlers">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Post(string route, Middleware<HttpContext>[] handlers, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Post(handlers);

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handler">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Put(string route, Func<HttpContext, HttpContext> handler, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Put(handler);

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handlers">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Put(string route, Middleware<HttpContext>[] handlers, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Put(handlers);

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handler">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Delete(string route, Func<HttpContext, HttpContext> handler, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Delete(handler);

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handlers">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Delete(string route, Middleware<HttpContext>[] handlers, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Delete(handlers);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handler">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Options(string route, Func<HttpContext, HttpContext> handler, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Options(handler);

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handlers">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void Options(string route, Middleware<HttpContext>[] handlers, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).Options(handlers);

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handler">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void All(string route, Func<HttpContext, HttpContext> handler, OidcOptions[] security = null) => Route(route, new RouteOptions(security: security)).All(handler);

        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="route">The path to match on.</param>
        /// <param name="handlers">The handler to run.</param>
        /// <param name="security">Security rules to override API-level security.</param>
        public void All(string route, Middleware<HttpContext>[] handlers, OidcOptions[] security = null) => Route(route, new RouteOptions(security)).All(handlers);

        /// <summary>
        /// Create a new route on a specified path.
        /// </summary>
        /// <returns>An ApiRoute that handlers can be added to.</returns>
        /// <param name="path">The path to match on.</param>
        public ApiRoute Route(string path)
        {
            return new ApiRoute(this, this.Opts.BasePath + path, new RouteOptions(security: this.Opts.Security));
        }

        /// <summary>
        /// Create a new route on a specified path.
        /// </summary>
        /// <returns>An ApiRoute that handlers can be added to.</returns>
        /// <param name="path">The path to match on.</param>
        /// <param name="options">Optional middleware and security rules to apply to the route.</param>
        public ApiRoute Route(string path, RouteOptions options)
        {
            return new ApiRoute(this, this.Opts.BasePath + path, options);
        }

        internal override BaseResource Register()
        {
            var resource = new NitricResource { Name = this.Name, Type = ResourceType.Api };
            var apiResource = new ProtoApiResource();

            foreach (var oidcOption in this.Opts.Security)
            {
                this.AttachOidc(oidcOption);

                var scopes = new ApiScopes();
                scopes.Scopes.Add(oidcOption.Scopes);
                apiResource.Security.Add(oidcOption.Name, scopes);
            }

            var request = new ResourceDeclareRequest { Id = resource, Api = apiResource };
            client.Declare(request);

            return this;
        }
    }

    public class RouteOptions
    {
        // The middleware that is run on every route
        public Middleware<HttpContext>[] Middlewares { get; set; }

        // Security rules to apply to this specific route
        public OidcOptions[] Security { get; set; }

        public RouteOptions(
            OidcOptions[] security = null,
            Middleware<HttpContext>[] middleware = null
        )
        {
            this.Security = security ?? Array.Empty<OidcOptions>();
            this.Middlewares = middleware ?? new Middleware<HttpContext>[] { };
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
            }
            return this.Opts.Middlewares.Append(ComposedMiddleware).ToArray();
        }

        private Middleware<HttpContext>[] ConcatMiddleware(Middleware<HttpContext>[] middlewares)
        {
            return this.Opts.Middlewares.Concat(middlewares).ToArray();
        }

        /// <summary>
        /// Create a new GET handler on the specified route.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        public void Get(Func<HttpContext, HttpContext> handler) => Method(this.Path, new HttpMethod[] { HttpMethod.Get }, this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new GET middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers">The handler to run.</param>
        public void Get(params Middleware<HttpContext>[] handlers) => Method(this.Path, new HttpMethod[] { HttpMethod.Get }, this.Opts, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new POST handler on the specified route.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        public void Post(Func<HttpContext, HttpContext> handler) => Method(this.Path, new HttpMethod[] { HttpMethod.Post }, this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new POST middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers">The handler to run.</param>
        public void Post(params Middleware<HttpContext>[] handlers) => Method(this.Path, new HttpMethod[] { HttpMethod.Post }, this.Opts, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new PUT handler on the specified route.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        public void Put(Func<HttpContext, HttpContext> handler) => Method(this.Path, new HttpMethod[] { HttpMethod.Put }, this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new PUT middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers">The handler to run.</param>
        public void Put(params Middleware<HttpContext>[] handlers) => Method(this.Path, new HttpMethod[] { HttpMethod.Post }, this.Opts, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new DELETE handler on the specified route.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        public void Delete(Func<HttpContext, HttpContext> handler) => Method(this.Path, new HttpMethod[] { HttpMethod.Delete }, this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new DELETE middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers">The handler to run.</param>
        public void Delete(params Middleware<HttpContext>[] handlers) => Method(this.Path, new HttpMethod[] { HttpMethod.Delete }, this.Opts, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        public void Options(Func<HttpContext, HttpContext> handler) => Method(this.Path, new HttpMethod[] { HttpMethod.Options }, this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new OPTIONS middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers">The handler to run.</param>
        public void Options(params Middleware<HttpContext>[] handlers) => Method(this.Path, new HttpMethod[] { HttpMethod.Options }, this.Opts, ConcatMiddleware(handlers));

        /// <summary>
        /// Create a new OPTIONS handler on the specified route.
        /// </summary>
        /// <param name="handler"></param>
        public void Patch(Func<HttpContext, HttpContext> handler) => Method(this.Path, new HttpMethod[] { HttpMethod.Patch }, this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new OPTIONS middleware chain on the specified route.
        /// </summary>
        /// <param name="handlers"></param>
        public void Patch(params Middleware<HttpContext>[] handlers) => Method(this.Path, new HttpMethod[] { HttpMethod.Patch }, this.Opts, ConcatMiddleware(handlers));

        HttpMethod[] httpMethods = new HttpMethod[]
        {
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Delete,
            HttpMethod.Head,
            HttpMethod.Options,
            HttpMethod.Patch
        };
        /// <summary>
        /// Create a new handler on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        public void All(Func<HttpContext, HttpContext> handler) => Method(this.Path, (HttpMethod[])Enum.GetValues(typeof(HttpMethod)), this.Opts, ConcatMiddleware(handler));

        /// <summary>
        /// Create a new chain of middleware on the specified route for every HTTP verb.
        /// </summary>
        /// <param name="handlers">The handler to run.</param>
        public void All(params Middleware<HttpContext>[] handlers) => Method(this.Path, (HttpMethod[])Enum.GetValues(typeof(HttpMethod)), this.Opts, ConcatMiddleware(handlers));

        internal void Method(string route, HttpMethod[] methods, RouteOptions options, Middleware<HttpContext>[] middlewares)
        {
            var opts = new ApiWorkerOptions
            {
                SecurityDisabled = options.Security.Count() == 0
            };

            foreach (var oidcOption in options.Security)
            {
                var scopes = new ApiWorkerScopes();
                scopes.Scopes.Add(oidcOption.Scopes);
                opts.Security.Add(oidcOption.Name, scopes);
                this.api.AttachOidc(oidcOption);
            }

            var registrationRequest = new RegistrationRequest
            {
                Api = this.api.Name,
                Options = opts,
                Path = route,
            };

            registrationRequest.Methods.AddRange(methods.Select((method) => method.Method).ToHashSet());

            var orderedMiddlewares = middlewares.Reverse().ToArray();

            var apiWorker = new ApiWorker(registrationRequest, orderedMiddlewares);

            Nitric.RegisterWorker(apiWorker);
        }
    }
}