using System;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Nitric.Proto.Secret.v1;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;
namespace Nitric.Api.Secret
{
    public class Secret
    {
        public readonly static string LATEST = "latest";

        internal GrpcClient client;
        public string Name { get; private set; }

        public Secret(GrpcClient client, string name)
        {
            this.client = client;
            this.Name = name;
        }
        public SecretVersion Version(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(version);
            }
            return new SecretVersion(this, version);
        }
        public SecretVersion Latest()
        {
            return new SecretVersion(this, LATEST);
        }
        public SecretVersion Put(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentNullException("provide non-empty value");
            }
            var request = new SecretPutRequest
            {
                Secret = new Proto.Secret.v1.Secret { Name = this.Name },
                Value = Google.Protobuf.ByteString.CopyFrom(value),
            };
            var secretResponse = client.Put(request);
            return new SecretVersion(
                new Secret(
                    this.client,
                    secretResponse.SecretVersion.Secret.Name
                ),
                secretResponse.SecretVersion.Version
            );
        }
        public SecretVersion Put(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(value);
            }
            return Put(Encoding.UTF8.GetBytes(value));
        }
        public override string ToString()
        {
            return "[name=" + this.Name + "]";
        }
    }
}
