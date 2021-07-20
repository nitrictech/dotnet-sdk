using System;
using System.Collections.Generic;
namespace Nitric.Api.Document
{
    public class Document<T> where T : IDictionary<string, object>, new()
    {
        public DocumentRef<T> Ref { get; private set; }
        public T Content { get; private set; }

        internal Document(DocumentRef<T> documentRef, T content)
        {
            this.Ref = documentRef;
            this.Content = content;
        }
    }
}
