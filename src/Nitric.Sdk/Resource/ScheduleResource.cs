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
using Nitric.Proto.Resources.v1;
using NitricResource = Nitric.Proto.Resources.v1.ResourceIdentifier;
using ResourceType = Nitric.Proto.Resources.v1.ResourceType;

namespace Nitric.Sdk.Resource
{
    public class ScheduleResource : BaseResource
    {
        internal ScheduleResource(string name) : base(name, ResourceType.Schedule)
        {

        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            BaseResource.client.Declare(request);
            return this;
        }

        /// <summary>
        /// Run code at a specific time, represented by a rate and a frequency. e.g. every 7 days
        /// </summary>
        /// <param name="rate">The interval for the schedule running. e.g. '7 days', '1 hour', '5 minutes'</param>
        /// <param name="middleware">The middleware (code) to run on a schedule.</param>
        public void Every(string rate, Func<IntervalContext, IntervalContext> middleware)
        {
            var registration = new RegistrationRequest
            {
                ScheduleName = this.Name,
                Every = new ScheduleEvery { Rate = rate }
            };

            var worker = new ScheduleWorker(registration, middleware);

            Nitric.RegisterWorker(worker);
        }

        /// <summary>
        /// Run code at a specific time, represented by a rate and a frequency. e.g. every 7 days
        /// </summary>
        /// <param name="rate">The interval for the schedule running. e.g. '7 days', '1 hour', '5 minutes'</param>
        /// <param name="middleware">The middlewares (code) to run on a schedule.</param>
        public void Every(string rate, params Middleware<IntervalContext>[] middleware)
        {
            var registration = new RegistrationRequest
            {
                ScheduleName = this.Name,
                Every = new ScheduleEvery { Rate = rate }
            };
            var worker = new ScheduleWorker(registration, middleware);

            Nitric.RegisterWorker(worker);
        }

        /// <summary>
        /// Run code at a specific time, represented by a cron expression.
        /// </summary>
        /// <param name="expression">The cron expression representing when the schedule should run.</param>
        /// <param name="middleware">The middleware (code) to run on a schedule.</param>
        public void Cron(string expression, Func<IntervalContext, IntervalContext> middleware)
        {
            var registration = new RegistrationRequest
            {
                ScheduleName = this.Name,
                Cron = new ScheduleCron { Expression = expression }
            };
            var worker = new ScheduleWorker(registration, middleware);

            Nitric.RegisterWorker(worker);
        }

        /// <summary>
        /// Run code at a specific time, represented by a cron expression.
        /// </summary>
        /// <param name="expression">The cron expression representing when the schedule should run.</param>
        /// <param name="middleware">The middlewares (code) to run on a schedule.</param>
        public void Cron(string expression, params Middleware<IntervalContext>[] middleware)
        {
            var registration = new RegistrationRequest
            {
                ScheduleName = this.Name,
                Cron = new ScheduleCron { Expression = expression }
            };
            var worker = new ScheduleWorker(registration, middleware);

            Nitric.RegisterWorker(worker);
        }
    }
}
