using System.Threading;
using System.Threading.Tasks;
using Nitric.Sdk.Common;
using Nitric.Proto.Websockets.v1;
using GrpcClient = Nitric.Proto.Websockets.v1.WebsocketHandler.WebsocketHandlerClient;
using Nitric.Sdk.Service;
using System;

namespace Nitric.Sdk.Worker
{
    public class WebsocketWorker : AbstractWorker<WebsocketContext>
    {
        readonly private RegistrationRequest RegistrationRequest;

        public WebsocketWorker(RegistrationRequest request, Func<WebsocketContext, WebsocketContext> middleware) : base(middleware)
        {
            this.RegistrationRequest = request;
        }

        public WebsocketWorker(RegistrationRequest request, params Middleware<WebsocketContext>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
        }

        public override async Task Start()
        {
            var client = new GrpcClient(GrpcChannelProvider.GetChannel());

            var stream = client.HandleEvents();

            await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var req = stream.ResponseStream.Current;

                if (req.RegistrationResponse != null)
                {
                    // Websocket connected with Nitric server.
                }

                var ctx = WebsocketContext.FromRequest(req);

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

            await stream.RequestStream.CompleteAsync();
        }
    }
}

