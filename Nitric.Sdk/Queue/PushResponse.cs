using System.Collections.Generic;

namespace Nitric.Api.Queue
{
    public class PushResponse
    {
        private readonly List<FailedTask> failedTasks;
        public PushResponse(List<FailedTask> failedTasks)
        {
            this.failedTasks = failedTasks;
        }
        public List<FailedTask> getFailedTasks()
        {
            return failedTasks;
        }
    }
}
