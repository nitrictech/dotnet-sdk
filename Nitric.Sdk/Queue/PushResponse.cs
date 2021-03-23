using System.Collections.Generic;

namespace Nitric.Api.Queue
{
    public class PushResponse
    {
        private readonly List<FailedEvent> failedEvents;
        public PushResponse(List<FailedEvent> failedEvents)
        {
            this.failedEvents = failedEvents;
        }
        public List<FailedEvent> getFailedEvents()
        {
            return failedEvents;
        }
    }
}
