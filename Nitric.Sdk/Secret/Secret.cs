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
using System.Text;
using Nitric.Proto.Secret.v1;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;
namespace Nitric.Api.Secret
{
    public class Secret
    {
        public readonly static string LATEST = "latest";

        internal GrpcClient client;
        public readonly string Name;

        internal Secret(GrpcClient client, string name)
        {
            this.client = client;
            this.Name = name;
        }
        public SecretVersion Version(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("version");
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
                throw new ArgumentNullException("value");
            }
            var request = new SecretPutRequest
            {
                Secret = new Proto.Secret.v1.Secret { Name = this.Name },
                Value = Google.Protobuf.ByteString.CopyFrom(value),
            };
            try
            {
                var secretResponse = client.Put(request);
                return new SecretVersion(
                    new Secret(
                        this.client,
                        secretResponse.SecretVersion.Secret.Name
                    ),
                    secretResponse.SecretVersion.Version
                );
            }
            catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.FromRpcException(re);
            }
        }
        public SecretVersion Put(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            return Put(Encoding.UTF8.GetBytes(value));
        }
        public override string ToString()
        {
            return "[name=" + this.Name + "]";
        }
    }
}
