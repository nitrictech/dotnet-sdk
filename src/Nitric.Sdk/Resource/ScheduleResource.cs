// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using Nitric.Sdk.Service;
using Nitric.Sdk.Worker;
using Nitric.Proto.Schedules.v1;

namespace Nitric.Sdk.Resource
{
    public class ScheduleResource
    {
        private string Name;

        internal ScheduleResource(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Run code at a specific time, represented by a rate and a frequency. e.g. every 7 days
        /// </summary>
        /// <param name="rate">The interval for the schedule running. e.g. 7 for every '7 days'</param>
        /// <param name="frequency">The frequency for the schedule running. e.g. Days for every '7 days'</param>
        /// <param name="middleware">The middleware (code) to run on a schedule.</param>
        public void Every(int rate, Frequency frequency, Func<IntervalContext, IntervalContext> middleware)
        {
            var registration = new RegistrationRequest
            {
                ScheduleName = this.Name,
                Every = new ScheduleEvery { Rate = rate, }
            };
            var worker = new ScheduleWorker();

            Nitric.RegisterWorker(faas);
        }

        /// <summary>
        /// Run code at a specific time, represented by a rate and a frequency. e.g. every 7 days
        /// </summary>
        /// <param name="rate">The interval for the schedule running. e.g. 7 for every '7 days'</param>
        /// <param name="frequency">The frequency for the schedule running. e.g. Days for every '7 days'</param>
        /// <param name="middleware">The middlewares (code) to run on a schedule.</param>
        public void Every(int rate, Frequency frequency, params Middleware<IntervalContext>[] middleware)
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

        /// <summary>
        /// Run code at a specific time, represented by a cron expression.
        /// </summary>
        /// <param name="expression">The cron expression representing when the schedule should run.</param>
        /// <param name="middleware">The middleware (code) to run on a schedule.</param>
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

        /// <summary>
        /// Run code at a specific time, represented by a cron expression.
        /// </summary>
        /// <param name="expression">The cron expression representing when the schedule should run.</param>
        /// <param name="middleware">The middlewares (code) to run on a schedule.</param>
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
