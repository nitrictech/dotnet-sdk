using System;
namespace Nitric.Faas
{
    public class TopicResponseContext : ResponseContext
    {
        public bool Success { get; set; }

        public TopicResponseContext SetSuccess(bool success)
        {
            this.Success = Success;
            return this;
        }
    }
}
