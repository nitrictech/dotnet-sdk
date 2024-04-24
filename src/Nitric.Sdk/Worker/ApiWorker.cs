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

using System.Threading;
using System.Threading.Tasks;
using Nitric.Sdk.Common;
using Nitric.Proto.Apis.v1;
using GrpcClient = Nitric.Proto.Apis.v1.Api.ApiClient;
using Nitric.Sdk.Service;
using System;

namespace Nitric.Sdk.Worker
{
    public class ApiWorker : AbstractWorker<HttpContext>
    {
        readonly private RegistrationRequest RegistrationRequest;
        public GrpcClient GrpcClient { private get; set; }

        public ApiWorker(RegistrationRequest request, Func<HttpContext, HttpContext> middleware) : base(middleware)
        {
            this.RegistrationRequest = request;
            this.GrpcClient = new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        public ApiWorker(RegistrationRequest request, params Middleware<HttpContext>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
            this.GrpcClient = new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        public override async Task Start(CancellationToken cancellationToken = default)
        {
            var stream = this.GrpcClient.Serve();

            await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

            while (await stream.ResponseStream.MoveNext(cancellationToken))
            {
                var req = stream.ResponseStream.Current;

                if (req.HttpRequest != null)
                {
                    var ctx = HttpContext.FromRequest(req);

                    try
                    {
                        ctx = this.Middleware(ctx);
                    }
                    catch (Exception err)
                    {
                        ctx.Res.WithError(err);
                    }

                    await stream.RequestStream.WriteAsync(ctx.ToResponse());
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            await stream.RequestStream.CompleteAsync();
        }
    }
}

