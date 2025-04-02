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

namespace Nitric.Sdk.Resource
{
    public delegate OidcOptions OidcScopes(params string[] scopes);

    public class OidcOptions
    {
        public string Name { get; set; }
        public string Issuer { get; set; }
        public string[] Audiences { get; set; }
        public string[] Scopes { get; set; }

        public OidcOptions(string name, string issuer, string[] audiences, string[] scopes)
        {
            this.Name = name;
            this.Issuer = issuer;
            this.Audiences = audiences;
            this.Scopes = scopes;
        }
    }

    public class OidcResource : BaseResource
    {
        private string ApiName { get; set; }
        private string Issuer { get; set; }
        private string[] Audiences { get; set; }
        private string RuleName { get; set; }

        public OidcResource(string name, string apiName, OidcOptions options) : base(name, ResourceType.ApiSecurityDefinition)
        {
            this.ApiName = apiName;
            this.Issuer = options.Issuer;
            this.Audiences = options.Audiences;
            this.RuleName = options.Name;
        }

        internal override BaseResource Register()
        {
            var resource = new ResourceIdentifier
            {
                Name = this.RuleName,
                Type = ResourceType.ApiSecurityDefinition,
            };

            var oidcRes = new ApiOpenIdConnectionDefinition
            {
                Issuer = this.Issuer,
            };
            oidcRes.Audiences.AddRange(this.Audiences);

            var secDef = new ApiSecurityDefinitionResource
            {
                ApiName = this.ApiName,
                Oidc = oidcRes
            };

            var request = new ResourceDeclareRequest { Id = resource, ApiSecurityDefinition = secDef };
            BaseResource.client.Declare(request);

            return this;
        }
    }
}