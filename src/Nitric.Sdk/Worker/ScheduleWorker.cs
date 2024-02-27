using System.Threading;
using System.Threading.Tasks;
using Nitric.Sdk.Common;
using Nitric.Proto.Schedules.v1;
using GrpcClient = Nitric.Proto.Schedules.v1.Schedules.SchedulesClient;
using Nitric.Sdk.Service;
using System;

namespace Nitric.Sdk.Worker
{
    public class ScheduleWorker : AbstractWorker<IntervalContext>
    {
        readonly private RegistrationRequest RegistrationRequest;

        public ScheduleWorker(RegistrationRequest request, Func<IntervalContext, IntervalContext> middleware) : base(middleware)
        {
            this.RegistrationRequest = request;
        }

        public ScheduleWorker(RegistrationRequest request, params Middleware<IntervalContext>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
        }

        public override async Task Start()
        {
            var client = new GrpcClient(GrpcChannelProvider.GetChannel());

            var stream = client.Schedule();

            await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var req = stream.ResponseStream.Current;

                if (req.RegistrationResponse != null)
                {
                    // Schedule connected with Nitric server.
                }

                var ctx = IntervalContext.FromRequest(req);

                try
                {
                    ctx = this.Middleware(ctx);
                }
                catch (Exception err)
                {
                    Console.WriteLine("Unhandled application error: {0}", err.ToString());
                }

                await stream.RequestStream.WriteAsync(ctx.ToResponse());
            }

            await stream.RequestStream.CompleteAsync();
        }
    }
}

