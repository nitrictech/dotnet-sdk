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
    /// <summary>
    /// The value of a specific version of a secret.
    /// </summary>
    public class SecretValue
    {
        /// <summary>
        /// The version containing this value.
        /// </summary>
        public readonly SecretVersion SecretVersion;

        /// <summary>
        /// The value retrieved from the secrets store as bytes.
        /// </summary>
        public byte[] ValueBytes { get; }

        /// <summary>
        /// The the value retrieved from the secrets store as a string.
        /// </summary>
        public string Value { get; }

        internal SecretValue(SecretVersion secretVersion, byte[] value)
        {
            this.SecretVersion = secretVersion;
            this.Value = Encoding.UTF8.GetString(value);
            this.ValueBytes = value;
        }

        /// <summary>
        /// Returns a string representation of this secret value. Does not contain the secret details for safety.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return GetType().Name
                   + "[secretVersion=" + this.SecretVersion
                   + ", value.length=" + Value.Length
                   + "]";
        }
    }
}
