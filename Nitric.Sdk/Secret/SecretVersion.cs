using System;
using Nitric.Proto.Secret.v1;
using System.Text;
namespace Nitric.Api.Secret
{
    public class SecretVersion
    {
        public readonly Secret Secret;
        public readonly string Version;

        public SecretVersion(Secret secret, string version)
        {
            if (secret == null || string.IsNullOrEmpty(secret.Name))
            {
                throw new ArgumentNullException(secret.Name);
            }
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(version);
            }
            this.Secret = secret;
            this.Version = version;
        }
        public byte[] Access()
        {
            var secret = new SecretAccessRequest
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret { Name = this.Secret.Name },
                    Version = this.Version,
                }
            };
            var response = this.Secret.client.Access(secret);
            return response.Value.ToByteArray();
        }
        public string AccessText()
        {
            return Encoding.UTF8.GetString(Access());
        }
        public override string ToString()
        {
            return GetType().Name
                + "[secret=" + this.Secret
                + ", version=" + this.Version
                + "]";
        }
    }
}
