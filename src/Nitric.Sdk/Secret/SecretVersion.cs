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
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Secret
{
    /// <summary>
    /// A reference to a specific version of a secret.
    /// </summary>
    public class SecretVersion
    {
        /// <summary>
        /// The secret this version refers to.
        /// </summary>
        public readonly Secret Secret;

        /// <summary>
        /// The unique id of this version.
        /// </summary>
        public readonly string Id;

        internal SecretVersion(Secret secret, string id)
        {
            if (secret == null || string.IsNullOrEmpty(secret.Name))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            this.Secret = secret;
            this.Id = id;
        }

        /// <summary>
        /// Retrieve the value stored in this version.
        /// </summary>
        /// <returns>The secret value from the secrets store.</returns>
        /// <exception cref="NitricException"></exception>
        public SecretValue Access()
        {
            var secret = new SecretAccessRequest
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret { Name = this.Secret.Name },
                    Version = this.Id,
                }
            };
            try
            {
                var response = this.Secret.Client.Access(secret);
                var value = response.Value.ToByteArray();
                //Return a new secret value with a reference to this secret version
                return new SecretValue(
                    this,
                    value is { Length: > 0 } ? value : Array.Empty<byte>()
                );
            }
            catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Return a string representation of this secret version. Will not contain secret values.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return GetType().Name
                   + "[secret=" + this.Secret
                   + ", version=" + this.Id
                   + "]";
        }
    }
}
