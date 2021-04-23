using System;
namespace Nitric.Api.Event
{
    public class Topic
    {
        public string Name { get; private set; }
        public Topic(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return GetType().Name + "[name=" + Name + "]";
        }
        public class Builder
        {
            string name;

            public Builder Name(string name)
            {
                this.name = name;
                return this;
            }
            public Topic Build()
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                return new Topic(name);
            }
            public Topic Build(string name)
            {
                return Name(name).Build();
            }
        }
    }
}
