using System;
using Google.Type;
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
            var faas = new Faas(new ScheduleRateWorkerOptions
            {
                Rate = rate,
                Frequency = frequency,
                Description = this.name
            });

            faas.Event(middleware);

            Nitric.RegisterWorker(faas);
        }

        public void Every(int rate, Frequency frequency, params Middleware<EventContext>[] middleware)
        {
            var faas = new Faas(new ScheduleRateWorkerOptions
            {
                Rate = rate,
                Frequency = frequency,
                Description = this.name
            });

            faas.Event(middleware);

            Nitric.RegisterWorker(faas);
        }

        public void Cron(string expression, Func<EventContext, EventContext> middleware)
        {
            var faas = new Faas(new ScheduleCronWorkerOptions
            {
                Description = this.name,
                Cron = expression
            });

            faas.Event(middleware);

            Nitric.RegisterWorker(faas);
        }

        public void Cron(string expression, params Middleware<EventContext>[] middleware)
        {
            var faas = new Faas(new ScheduleCronWorkerOptions
            {
                Description = this.name,
                Cron = expression
            });

            faas.Event(middleware);

            Nitric.RegisterWorker(faas);
        }
    }
}
