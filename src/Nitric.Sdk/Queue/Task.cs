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
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Nitric.Proto.Queue.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Queue
{
    /// <summary>
    /// Represents a Message to be delivered via a Queue.
    /// </summary>
    public class Task
    {
        public string Id { get; set; }
        public string PayloadType { get; set; }
        public object Payload { get; set; }

        public Task()
        {
        }

        /// <summary>
        /// Return a string representation of the task.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return GetType().Name + "[ID=" + this.Id + "]";
        }

        internal NitricTask ToWire()
        {
            Struct payload = null;
            if (this.Payload != null)
            {
                var jsonPayload = JsonConvert.SerializeObject(this.Payload);
                payload = JsonParser.Default.Parse<Struct>(jsonPayload);
            }

            return new NitricTask
            {
                Id = this.Id ?? "",
                PayloadType = this.PayloadType ?? "",
                Payload = payload
            };
        }
    }

    /// <summary>
    /// Represents a task received locally for processing.
    ///
    /// Since received tasks are on a limited time lease they include a lease ID.
    /// Received tasks must be completed to be removed from the queue and avoid reprocessing
    /// </summary>
    public class ReceivedTask : Task
    {
        /// <summary>
        /// The queue that was the source of this task.
        /// </summary>
        public Queue Queue { get; set; }

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
            if (string.IsNullOrEmpty(this.LeaseId))
            {
                throw new ArgumentNullException(nameof(this.LeaseId));
            }

            var request = new QueueCompleteRequest
            {
                Queue = this.Queue.Name,
                LeaseId = this.LeaseId,
            };
            try
            {
                Queue.QueuesClient.Client.Complete(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw Common.NitricException.FromRpcException(re);
            }
        }
    }
}
