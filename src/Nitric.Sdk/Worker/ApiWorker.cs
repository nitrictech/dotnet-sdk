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

        public ApiWorker(RegistrationRequest request, Func<HttpContext, HttpContext> middleware) : base(middleware)
        {
            this.RegistrationRequest = request;
        }

        public ApiWorker(RegistrationRequest request, params Middleware<HttpContext>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
        }

        public override async Task Start()
        {
            var client = new GrpcClient(GrpcChannelProvider.GetChannel());

            var stream = client.Serve();

            await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var req = stream.ResponseStream.Current;


                if (req.RegistrationResponse != null)
                {
                    // Schedule connected with Nitric server.
                }

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

            await stream.RequestStream.CompleteAsync();
        }
    }
}

