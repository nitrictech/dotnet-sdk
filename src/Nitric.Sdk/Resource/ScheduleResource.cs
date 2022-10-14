using Nitric.Sdk.Function;

namespace Nitric.Sdk.Resource
{
    public class ScheduleResource
    {
        private string name;

        public ScheduleResource(string name)
        {
            this.name = name;
        }

        public void Every(int rate, Frequency frequency, IHandler<EventContext> middleware)
        {

        }
    }
}
