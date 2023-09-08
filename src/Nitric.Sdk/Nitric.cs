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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nitric.Sdk.Function;
using Nitric.Sdk.Resource;

namespace Nitric.Sdk
{
    /// <summary>
    /// Nitric SDK entrypoint with helper methods for requesting and working with cloud resources.
    /// </summary>
    public class Nitric
    {
        private static readonly List<Faas> Workers = new List<Faas>();

        private static readonly Dictionary<Type, Dictionary<string, BaseResource>> Cache =
            new Dictionary<Type, Dictionary<string, BaseResource>>();

        internal static void RegisterWorker(Faas worker)
        {
            Nitric.Workers.Add(worker);
        }

        /// <summary>
        /// Start running the Nitric application.
        ///
        /// This is typically called after registering all required resources, APIs, Schedules and Subscribers.
        /// </summary>
        public static void Run()
        {
            // Start each instance in a new thread and wait until they complete.
            Task.WaitAll(Workers.Select(worker => worker.Start()).ToArray());
        }

        /// <summary>
        /// Declare a REST/HTTP API resource.
        ///
        /// Used to register routes and handlers for HTTP requests.
        /// </summary>
        /// <param name="name">The unique name of this API.</param>
        /// <returns></returns>
        public static ApiResource Api(string name, ApiOptions options = null)
        {
            return new ApiResource(name, options);
        }

        /// <summary>
        /// Declare a schedule.
        /// </summary>
        /// <param name="description">A short description of the schedule</param>
        /// <returns></returns>
        public static ScheduleResource Schedule(string description)
        {
            return new ScheduleResource(description);
        }

        private static T Cached<T>(string name, Func<string, T> make) where T : BaseResource
        {
            var typeMap = Cache.GetValueOrDefault(typeof(T), new Dictionary<string, BaseResource>());
            var resource = typeMap!.GetValueOrDefault(name, make(name)) as T;

            // update the cache
            if (Cache.ContainsKey(typeof(T)))
            {
                Cache[typeof(T)] = typeMap;
            }
            else
            {
                Cache.Add(typeof(T), typeMap);

            }

            return resource;
        }

        /// <summary>
        /// Declare a bucket resources for file/blob storage.
        /// </summary>
        /// <param name="name">The unique name of the bucket within this application.</param>
        /// <returns>A bucket resource, if the name has already been declared the same resource will be returned.</returns>
        public static BucketResource Bucket(string name) => Cached(name, t => new BucketResource(t));


        /// <summary>
        /// Declare a collection resource for document storage.
        /// </summary>
        /// <param name="name">The unique name of the collection within this application.</param>
        /// <typeparam name="T">The type of documents to be stored in the collection.</typeparam>
        /// <returns>A collection resource, if the name has already been declared the same resource will be returned.</returns>
        public static CollectionResource<T> Collection<T>(string name) =>
            Cached(name, t => new CollectionResource<T>(t));

        /// <summary>
        /// Declare a secret resource for accessing and putting secret values.
        /// </summary>
        /// <param name="name">The unique name of the secret within this application.</param>
        /// <returns>A secret resource, if the name has already been declared the same resource will be returned.</returns>
        public static SecretResource Secret(string name) => Cached(name, t => new SecretResource(t));

        /// <summary>
        /// Declare a queue resources for pull-based tasks and batch workloads.
        /// </summary>
        /// <param name="name">The unique name of the queue within this application.</param>
        /// <returns>A queue resource, if the name has already been declared the same resource will be returned.</returns>
        public static QueueResource Queue(string name) => Cached(name, n => new QueueResource(n));

        /// <summary>
        /// Declare a topic resource for push-based events and messaging.
        /// </summary>
        /// <param name="name">The unique name of the topic within this application.</param>
        /// <returns>A topic resource, if the name has already been declared the same resource will be returned.</returns>
        public static TopicResource Topic(string name) => Cached(name, t => new TopicResource(t));
    }
}
