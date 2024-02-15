using System.Threading;
using System.Threading.Tasks;
using Nitric.Sdk.Common;
using Nitric.Proto.Schedules.v1;
using GrpcClient = Nitric.Proto.Schedules.v1.Schedules.SchedulesClient;
using Nitric.Sdk.Service;
using Nitric.Sdk.Resource;

namespace Nitric.Sdk.Worker
{
    public class ScheduleWorker : AbstractWorker<IntervalContext>
    {
        readonly private RegistrationRequest RegistrationRequest;

        public ScheduleWorker(RegistrationRequest request, params Middleware<IntervalContext>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
        }

        public override Task Start()
        {
            return Task.Run(async () =>
            {
                var client = new GrpcClient(GrpcChannelProvider.GetChannel());

                var stream = client.Schedule();

                await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

                while (!stream.ResponseStream.Current.Equals(null))
                {
                    var req = stream.ResponseStream.Current;

                    var ctx = IntervalContext.FromRequest(req);

                    ctx = this.Middleware(ctx);

                    await stream.RequestStream.WriteAsync(ctx.ToRequest());

                    await stream.ResponseStream.MoveNext(CancellationToken.None);
                }

                await stream.RequestStream.CompleteAsync();
            });
        }
    }
}

