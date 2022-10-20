using System;
using Nitric.Sdk.Function;

namespace Nitric.Sdk.Resource
{
    public class ScheduleResource
    {
        private string name;

        internal ScheduleResource(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Set the rate, frequency and handler for this schedule.
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="frequency"></param>
        /// <param name="middleware"></param>
        public void Every(int rate, Frequency frequency, Func<EventContext, EventContext> middleware)
        {

        }
    }
}
