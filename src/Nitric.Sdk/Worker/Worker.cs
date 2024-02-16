using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nitric.Sdk.Service;
using System.Linq;

namespace Nitric.Sdk.Worker
{
    interface IWorker
    {
        public Task Start();
    }

    public abstract class AbstractWorker<T> : IWorker
    {
        protected Func<T, T> Middleware;

        public AbstractWorker(params Middleware<T>[] middlewares)
        {
            if (middlewares.Count() == 0)
            {
                throw new ArgumentException("cannot create worker with no handlers");
            }

            // Convert the middleware array into a list
            var middlewareList = new List<Middleware<T>>();

            middlewareList.AddRange(middlewares);

            Func<T, T> lastCall = (context) => context;

            middlewares.Reverse();

            this.Middleware = middlewares.Aggregate(lastCall, (next, handler) =>
            {
                Func<T, T> nextFunc = (context) => handler(context, next) ?? context;

                return nextFunc;
            });
        }

        public AbstractWorker(Func<T, T> middleware)
        {
            this.Middleware = middleware;
        }

        public abstract Task Start();
    }
}

