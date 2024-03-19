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

