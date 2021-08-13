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
using AbstractClient = Nitric.Api.Common.AbstractClient;
using GrpcClient = Nitric.Proto.Secret.v1.SecretService.SecretServiceClient;
namespace Nitric.Api.Secret
{
    public class Secrets : AbstractClient
    {
        private GrpcClient secretServiceClient;
        public Secrets(GrpcClient client = null)
        {
            this.secretServiceClient = (client != null) ? client : new GrpcClient(this.GetChannel());
        }
        public Secret Secret(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return new Secret(this.secretServiceClient, name);
        }
    }
}
