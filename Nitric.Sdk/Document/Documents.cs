using System;
using System.Collections.Generic;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using Nitric.Api.Common;
namespace Nitric.Api.Document
{
    public class Documents : AbstractClient
    {
        private DocumentServiceClient documentClient;
        public Documents(DocumentServiceClient client = null)
        {
            this.documentClient = (client != null) ? client : new DocumentServiceClient(this.GetChannel());
        }

        public CollectionRef<T> Collection<T>(string name) where T : IDictionary<string, object>, new()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            return new CollectionRef<T>(this.documentClient, name);
        }
    }
}
