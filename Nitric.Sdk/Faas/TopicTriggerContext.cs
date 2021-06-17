using System;
namespace Nitric.Faas
{
    public class TopicTriggerContext : TriggerContext
    {
        public string Topic { get; private set; }
        public TopicTriggerContext(string topic)
        {
            this.Topic = topic;
        }
    }
}
