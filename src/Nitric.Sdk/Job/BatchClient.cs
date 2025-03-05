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
using GrpcClient = Nitric.Proto.Batch.v1.Batch.BatchClient;

namespace Nitric.Sdk.Job
{
    /// <summary>
    /// A batch client.
    /// </summary>
    public class BatchClient
    {
        internal readonly GrpcClient Client;

        /// <summary>
        /// Create a new batch client.
        /// </summary>
        /// <param name="client">Optional internal gRPC client to reuse.</param>
        public BatchClient(GrpcClient client = null)
        {
            this.Client = client ?? new GrpcClient(GrpcChannelProvider.GetChannel());
        }

        /// <summary>
        /// Create a reference to a job in a batch service.
        /// </summary>
        /// <param name="jobName">The name of the job.</param>
        /// <returns>The new job reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Job<T> Job<T>(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                throw new ArgumentNullException(nameof(jobName));
            }

            return new Job<T>(this, jobName);
        }
    }
}
