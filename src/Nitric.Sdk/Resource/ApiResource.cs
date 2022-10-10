using System;
using System.Text;
using Nitric.Proto.Resource.v1;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;
namespace Nitric.Sdk.Resource
{
    public class ApiResource : BaseResource
    {


        protected ApiResource(string name): base(name)
        {

        }

        private ApiResource Method(string path, string handler, params string[] methods)
        {
            return this;
        }

        public ApiResource Get(string path, string handler) => Method(path, handler, "GET");
        public ApiResource Post(string path, string handler) => Method(path, handler, "POST");
        public ApiResource Put(string path, string handler) => Method(path, handler, "PUT");
        public ApiResource Delete(string path, string handler) => Method(path, handler, "DELETE");
        public ApiResource Options(string path, string handler) => Method(path, handler, "OPTIONS");

        public ApiResource All(string path, string handler) => Method(path, handler, "");

        public override BaseResource Register()
        {
            // TODO: Implement me.
            throw new NotImplementedException();
        }
    }
}
