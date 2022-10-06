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
using Grpc.Core;

namespace Nitric.Api.Common
{
    public abstract class AbstractClient
    {
        string DefaultHostName = "127.0.0.1:50051";
        public AbstractClient()
        {
            GetChannel();
        }
        protected Channel GetChannel()
        {
            // TODO: Pull from settings
            string serviceBind = Util.GetEnvVar("SERVICE_BIND");
            if (string.IsNullOrEmpty(serviceBind))
            {
                serviceBind = DefaultHostName;
            }
            return new Channel(serviceBind, ChannelCredentials.Insecure); ;
        }
    }
}