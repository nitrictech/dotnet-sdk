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
using Nitric.Sdk.Common.Util;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;

namespace Nitric.Sdk.Secret
{
    /// <summary>
    /// The secrets service client.
    /// </summary>
    public class SecretsClient
    {
        private readonly GrpcClient secretServiceClient;

        /// <summary>
        /// Create a new secrets service client.
        /// </summary>
        /// <param name="client"></param>
        public SecretsClient(GrpcClient client = null)
        {
            this.secretServiceClient = (client != null) ? client : new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to a specific secret in the secret store.
        /// </summary>
        /// <param name="name">The name of the secret</param>
        /// <returns>The secret reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Secret Secret(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Secret(this.secretServiceClient, name);
        }
    }
}
