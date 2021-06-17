using System;
namespace Nitric.Faas
{
    public abstract class ResponseContext
    {
        public bool IsHttp()
        {
            return this.GetType() == typeof(HttpResponseContext);
        }
        public bool IsTopic()
        {
            return this.GetType() == typeof(TopicResponseContext);
        }
        public HttpResponseContext AsHttp()
        {
            if (this.IsHttp())
            {
                return this as HttpResponseContext;
            }
            return null;
        }
        public TopicResponseContext AsTopic()
        {
            if (this.IsTopic())
            {
                return this as TopicResponseContext;
            }
            return null;
        }
    }
}
