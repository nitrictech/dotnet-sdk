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
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;

namespace Nitric.Sdk.Secret
{
    /// <summary>
    /// Represents a secret in the secrets service.
    /// </summary>
    public class Secret
    {
        private const string LATEST = "latest";

        internal readonly GrpcClient Client;

        /// <summary>
        /// The name of the secret.
        /// </summary>
        public readonly string Name;

        internal Secret(GrpcClient client, string name)
        {
            this.Client = client;
            this.Name = name;
        }

        /// <summary>
        /// Create a reference to a specific version of the secret.
        /// </summary>
        /// <param name="version">The version Id</param>
        /// <returns>A secret version reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SecretVersion Version(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            return new SecretVersion(this, version);
        }

        /// <summary>
        /// Create reference which always points at the latest version of the secret.
        /// </summary>
        /// <returns>The secret reference.</returns>
        public SecretVersion Latest()
        {
            return new SecretVersion(this, LATEST);
        }

        /// <summary>
        /// Create a new version of this secret containing the provided value and set it as the latest version.
        /// </summary>
        /// <param name="value">The secret value to store from an array of bytes.</param>
        /// <returns>A reference to the specific version of the secret containing the provided value.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public SecretVersion Put(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var request = new SecretPutRequest
            {
                Secret = new Proto.Secret.v1.Secret { Name = this.Name },
                Value = Google.Protobuf.ByteString.CopyFrom(value),
            };
            try
            {
                var secretResponse = Client.Put(request);
                return new SecretVersion(
                    new Secret(
                        this.Client,
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

        /// <summary>
        /// Create a new version of this secret containing the provided value and set it as the latest version.
        /// </summary>
        /// <param name="value">The secret value to store from a string.</param>
        /// <returns>A reference to the specific version of the secret containing the provided value.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public SecretVersion Put(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            return Put(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// A string representing this secret. Will not contain the value of the secret.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[name=" + this.Name + "]";
        }
    }
}
