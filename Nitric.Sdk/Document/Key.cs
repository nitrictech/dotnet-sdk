using System;
using Collection = Nitric.Proto.Document.v1.Collection;
using GrpcKey = Nitric.Proto.Document.v1.Key;

namespace Nitric.Api.Document
{
    public class Key
    {
        Collection collection;
        string id;

        public Key(Collection collection, string id = null)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Provide non-null collection");
            }
            this.collection = collection;
            this.id = id != null ? id : "";
        }
        public GrpcKey ToKey()
        {
            return new GrpcKey
            {
                Collection = this.collection,
                Id = this.id,
            };
        }
        public override string ToString()
        {
            return this.GetType().Name + "[collection=" + collection + ", id=" + id + "]";
        }
    }
}
