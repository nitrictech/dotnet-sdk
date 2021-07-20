using System;
using DocumentServiceClient = Nitric.Proto.Document.v1.DocumentService.DocumentServiceClient;
using Collection = Nitric.Proto.Document.v1.Collection;
using GrpcKey = Nitric.Proto.Document.v1.Key;
using System.Collections.Generic;
namespace Nitric.Api.Document
{
    public class CollectionRef<T> where T : IDictionary<string, object>, new()
    {
        private Collection collection;
        private DocumentServiceClient documentClient;

        internal CollectionRef(DocumentServiceClient documentClient, string name, GrpcKey parentKey = null)
        {
            this.documentClient = documentClient;

            var collection = new Collection()
            {
                Name = name,
            };
            if (parentKey != null)
            {
                collection.Parent = parentKey;
            }
            this.collection = collection;
        }
        public DocumentRef<T> Doc(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
            {
                throw new ArgumentNullException(documentId);
            }
            return new DocumentRef<T>(
                this.documentClient,
                this.collection,
                documentId);
        }
        public Query<T> Query()
        {
            return new Query<T>(this.documentClient, this.collection);
        }
    }
}

