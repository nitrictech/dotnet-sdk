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
using SystemEnv = System.Environment;

namespace Nitric.Sdk.Common.Util
{
    /// <summary>
    /// Helper Utils for access common Nitric environment variables
    /// </summary>
    internal static class Environment
    {
        private const string NitricHostAddressDefault = "localhost:50051";
        private const string NitricHostAddressEnvVar = "SERVICE_ADDRESS";

        private static string GetEnvironmentVariable(string name, string defaultValue)
        {
            var envVar = SystemEnv.GetEnvironmentVariable(name);            
            return envVar != null && envVar.Length > 0 ? envVar : defaultValue;
        }

        internal static string GetNitricHost() =>
            "http://" + GetEnvironmentVariable(NitricHostAddressEnvVar, NitricHostAddressDefault);
    }
}
