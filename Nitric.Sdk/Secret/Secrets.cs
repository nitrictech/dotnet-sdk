using System;
using AbstractClient = Nitric.Api.Common.AbstractClient;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;
namespace Nitric.Api.Secret
{
    public class Secrets : AbstractClient
    {
        private GrpcClient secretServiceClient;
        public Secrets(GrpcClient client = null)
        {
            this.secretServiceClient = (client != null) ? client : new GrpcClient(this.GetChannel());
        }
        public Secret Secret(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            return new Secret(this.secretServiceClient, name);
        }
    }
}
