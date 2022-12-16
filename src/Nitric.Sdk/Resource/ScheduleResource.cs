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
