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
using Nitric.Proto.Resources.v1;
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.Resources.v1.Resources.ResourcesClient;
using ProtoResource = Nitric.Proto.Resources.v1.ResourceIdentifier;

namespace Nitric.Sdk.Resource
{
    public abstract class BaseResource
    {
        internal string Name;
        protected static GrpcClient client;
        protected ResourceType type;

        public BaseResource(string name, ResourceType type)
        {
            this.Name = name;
            this.type = type;
            client = (client == null) ? new GrpcClient(GrpcChannelProvider.GetChannel()) : client;
        }

        internal ProtoResource AsProtoResource()
        {
            return new ProtoResource { Name = this.Name, Type = this.type };
        }

        internal abstract BaseResource Register();
    }
}
