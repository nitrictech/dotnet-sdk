using System;
using Grpc.Core;

namespace Nitric.Api.Common
{
    public abstract class AbstractClient
    {
        public AbstractClient()
        {
            GetChannel();
        }
        protected Channel GetChannel()
        {
            // TODO: Pull from settings
            string serviceBind = Util.GetEnvVar("SERVICE_BIND");
            return new Channel(serviceBind, ChannelCredentials.Insecure); ;
        }
    }
}