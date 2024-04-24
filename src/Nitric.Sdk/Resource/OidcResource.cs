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