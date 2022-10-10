using System;
using System.Text;
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.Resource.v1.ResourceService.ResourceServiceClient;

namespace Nitric.Sdk.Resource
{
    public abstract class BaseResource : AbstractClient
    {
        protected string name;
        protected static GrpcClient client;

        protected BaseResource(string name)
        {
            this.name = name;
            BaseResource.client = (BaseResource.client == null) ? new GrpcClient(this.GetChannel()) : client;
        }

        public abstract BaseResource Register();
    }
}
