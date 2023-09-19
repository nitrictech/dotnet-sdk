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

using GrpcClient = Nitric.Proto.Queue.v1.QueueService.QueueServiceClient;
using Nitric.Sdk.Common;
using System;

namespace Nitric.Sdk.Queue
{
    /// <summary>
    /// A queues service client.
    /// </summary>
    public class QueuesClient
    {
        internal GrpcClient Client { get; private set; }

        /// <summary>
        /// Create a new queues service client.
        /// </summary>
        /// <param name="client"></param>
        public QueuesClient(GrpcClient client = null)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to the queue in the queues service.
        /// </summary>
        /// <param name="queueName">The queue's name</param>
        /// <returns>A new queue reference for sending or receiving tasks.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Queue<T> Queue<T>(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
            }

            return new Queue<T>(this, queueName);
        }
    }
}
