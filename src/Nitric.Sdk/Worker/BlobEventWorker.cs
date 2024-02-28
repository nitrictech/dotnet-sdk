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
using Nitric.Proto.Storage.v1;
using GrpcClient = Nitric.Proto.Storage.v1.StorageListener.StorageListenerClient;
using Nitric.Sdk.Service;
using Nitric.Sdk.Storage;
using System;

namespace Nitric.Sdk.Worker
{
    public class BlobEventWorker : AbstractWorker<BlobEventContext>
    {
        readonly private RegistrationRequest RegistrationRequest;

        public BlobEventWorker(RegistrationRequest request, Func<BlobEventContext, BlobEventContext> middleware) : base(middleware)
        {
            this.RegistrationRequest = request;
        }

        public BlobEventWorker(RegistrationRequest request, params Middleware<BlobEventContext>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
        }

        public override async Task Start()
        {
            var client = new GrpcClient(GrpcChannelProvider.GetChannel());

            var stream = client.Listen();

            await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var req = stream.ResponseStream.Current;

                if (req.RegistrationResponse != null)
                {
                    // Bucket listener connected with Nitric server.
                }
                else if (req.BlobEventRequest != null) 
                {
                    var ctx = BlobEventContext.FromRequest(req);

                    try
                    {
                        ctx = this.Middleware(ctx);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Unhandled application error: {0}", err.ToString());
                        ctx.Res.Success = false;
                    }

                    await stream.RequestStream.WriteAsync(ctx.ToResponse());
                }
            }

            await stream.RequestStream.CompleteAsync();
        }
    }
}

