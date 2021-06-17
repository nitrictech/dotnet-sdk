using TriggerRequest = Nitric.Proto.Faas.v1.TriggerRequest;

namespace Nitric.Faas
{
    public class Trigger
    {
        public byte[] Data { get; private set; }
        public string MimeType { get; private set; }
        public TriggerContext Context { get; private set; }

        private Trigger(byte[] data, string mimeType, TriggerContext context)
        {
            this.Data = data;
            this.MimeType = mimeType;
            this.Context = context;
        }
        public static Trigger FromGrpcTriggerRequest(TriggerRequest trigger)
        {
            TriggerContext ctx = TriggerContext.FromGrpcTriggerRequest(trigger);

            return new Trigger(
                trigger.Data.ToByteArray(),
                trigger.MimeType,
                ctx
            );
        }
        public Response DefaultResponse()
        {
            return this.DefaultResponse(null);
        }
        public Response DefaultResponse(byte[] data)
        {
            ResponseContext responseCtx = null;

            if (this.Context.IsHttp())
            {
                responseCtx = new HttpResponseContext();
            } else if (this.Context.IsTopic())
            {
                responseCtx = new TopicResponseContext();
            }

            return new Response(Data, responseCtx);
        }
    }
}
