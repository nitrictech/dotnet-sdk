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
using System.Text;
namespace Nitric.Sdk.Secret
{
    public class SecretValue
    {
        public readonly SecretVersion SecretVersion;
        public readonly byte[] Value;
        public readonly string ValueText;
        internal SecretValue(SecretVersion secretVersion, byte[] value)
        {
            this.SecretVersion = secretVersion;
            this.Value = value;
            this.ValueText = Encoding.UTF8.GetString(value);
        }
        public override string ToString()
        {
            return GetType().Name
                + "[secretVersion=" + this.SecretVersion
                + ", value.length=" + Value.Length
                + "]";
        }
    }
}
