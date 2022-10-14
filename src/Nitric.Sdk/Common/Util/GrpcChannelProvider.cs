using Grpc.Net.Client;

namespace Nitric.Sdk.Common.Util
{
    internal static class GrpcChannelProvider
    {
        private static GrpcChannel _channel;

        internal static GrpcChannel GetChannel()
        {
            if (GrpcChannelProvider._channel == null)
            {
                var address = Environment.GetNitricHost();
                GrpcChannelProvider._channel = GrpcChannel.ForAddress(address);
            }

            return GrpcChannelProvider._channel;
        }
    }
}
