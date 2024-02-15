using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nitric.Sdk.Service;
using System.Linq;

namespace Nitric.Sdk.Worker
{
    public abstract class AbstractWorker
    {
        protected Func<TriggerContext<TriggerRequest, TriggerResponse>, TriggerContext<TriggerRequest, TriggerResponse>> Middleware;

        public AbstractWorker(params Middleware<TriggerContext<TriggerRequest, TriggerResponse>>[] middlewares)
        {
            if (middlewares.Count() == 0)
            {
                throw new ArgumentException("cannot create schedule worker with no handlers");
            }

            // Convert the middleware array into a list
            var middlewareList = new List<Middleware<TriggerContext<TriggerRequest, TriggerResponse>>>();

            middlewareList.AddRange(middlewares);

            Func<TriggerContext<TriggerRequest, TriggerResponse>, TriggerContext<TriggerRequest, TriggerResponse>> lastCall = (context) => context;

            middlewares.Reverse();

            this.Middleware = middlewares.Aggregate(lastCall, (next, handler) =>
            {
                Func<TriggerContext<TriggerRequest, TriggerResponse>, TriggerContext<TriggerRequest, TriggerResponse>> nextFunc = (context) => handler(context, next) ?? context;

                return nextFunc;
            });
        }

        public abstract Task Start();
    }
}

