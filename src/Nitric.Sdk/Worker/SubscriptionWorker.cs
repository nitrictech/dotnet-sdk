using System.Threading;
using System.Threading.Tasks;
using Nitric.Sdk.Common;
using Nitric.Proto.Topics.v1;
using GrpcClient = Nitric.Proto.Topics.v1.Subscriber.SubscriberClient;
using Nitric.Sdk.Service;
using System;

namespace Nitric.Sdk.Worker
{
    public class SubscriptionWorker<T> : AbstractWorker<MessageContext<T>>
    {
        readonly private RegistrationRequest RegistrationRequest;

        public SubscriptionWorker(RegistrationRequest request, Func<MessageContext<T>, MessageContext<T>> middleware) : base(middleware)
        {
            this.RegistrationRequest = request;
        }

        public SubscriptionWorker(RegistrationRequest request, params Middleware<MessageContext<T>>[] middlewares) : base(middlewares)
        {
            this.RegistrationRequest = request;
        }

        public override async Task Start()
        {
            var client = new GrpcClient(GrpcChannelProvider.GetChannel());

            var stream = client.Subscribe();

            await stream.RequestStream.WriteAsync(new ClientMessage { RegistrationRequest = RegistrationRequest });

            Console.WriteLine(RegistrationRequest);

            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var req = stream.ResponseStream.Current;

                if (req.RegistrationResponse != null)
                {
                    // Topic connected with Nitric Server.
                }
                else if (req.MessageRequest != null)
                {
                    var ctx = MessageContext<T>.FromRequest(req);

                    ctx = this.Middleware(ctx);

                    await stream.RequestStream.WriteAsync(ctx.ToResponse());
                }
            }

            await stream.RequestStream.CompleteAsync();
        }
    }
}

