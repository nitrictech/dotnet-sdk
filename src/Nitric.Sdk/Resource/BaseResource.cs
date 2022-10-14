using System;
using System.Text;
using Nitric.Sdk.Common;
using Nitric.Sdk.Common.Util;
using GrpcClient = Nitric.Proto.Resource.v1.ResourceService.ResourceServiceClient;

namespace Nitric.Sdk.Resource
{
    public abstract class BaseResource
    {
        protected string name;
        protected static GrpcClient client;

        protected BaseResource(string name)
        {
            this.name = name;
            BaseResource.client = (BaseResource.client == null) ? new GrpcClient(GrpcChannelProvider.GetChannel()) : client;
        }

        public abstract BaseResource Register();
    }
}
