// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using Nitric.Proto.Secret.v1;
namespace Nitric.Api.Secret
{
    public class SecretVersion
    {
        public readonly Secret Secret;
        public readonly string Version;

        internal SecretVersion(Secret secret, string version)
        {
            if (secret == null || string.IsNullOrEmpty(secret.Name))
            {
                throw new ArgumentNullException("secret.Name");
            }
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("version");
            }
            this.Secret = secret;
            this.Version = version;
        }
        public SecretValue Access()
        {
            var secret = new SecretAccessRequest
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret { Name = this.Secret.Name },
                    Version = this.Version,
                }
            };
            try
            {
                var response = this.Secret.client.Access(secret);
                var value = response.Value.ToByteArray();
                //Return a new secret value with a reference to this secret version
                return new SecretValue(
                    this,
                    (value != null && value.Length > 0) ? value : new byte[0]
                );
            } catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.FromRpcException(re);
            }
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
