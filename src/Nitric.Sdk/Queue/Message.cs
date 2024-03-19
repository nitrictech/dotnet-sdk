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
using Nitric.Sdk.Common;
using Nitric.Proto.Queues.v1;
using System.Threading.Tasks;

namespace Nitric.Sdk.Queue
{
    /// <summary>
    /// Represents a message received locally for processing.
    ///
    /// Since received messages are on a limited time lease they include a lease ID.
    /// Received tasks must be completed to be removed from the queue and avoid reprocessing
    /// </summary>
    public class ReceivedMessage<T>
    {
        /// <summary>
        /// The queue that was the source of this task.
        /// </summary>
        internal Queue<T> Queue { get; set; }

        /// <summary>
        /// The message that was on the queue.
        /// </summary>
        public T Message { get; set; }

        /// <summary>
        /// The unique lease id for this task lease.
        /// </summary>
        public string LeaseId { get; set; }

        /// <summary>
        /// Complete this task and remove it from the source queue.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public void Complete()
        {
            var request = new QueueCompleteRequest
            {
                QueueName = this.Queue.Name,
                LeaseId = this.LeaseId,
            };

            try
            {
                Queue.Queues.Client.Complete(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Complete this task and remove it from the source queue.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NitricException"></exception>
        public async Task CompleteAsync()
        {
            var request = new QueueCompleteRequest
            {
                QueueName = this.Queue.Name,
                LeaseId = this.LeaseId,
            };

            try
            {
                await Queue.Queues.Client.CompleteAsync(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
    }

    /// <summary>
    /// Represents a task that was unable to be sent to a queue.
    /// </summary>
    public class FailedMessage<T>
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// The message that failed to be sent.
        /// </summary>
        public T Message { get; set; }

        /// <summary>
        /// Return a string representation of the failed task.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "FailedMessage[details=" + Details + "]";
        }
    }
}
