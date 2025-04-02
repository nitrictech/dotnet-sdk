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
using Nitric.Proto.Batch.v1;
using Nitric.Sdk.Common;
using GrpcClient = Nitric.Proto.Batch.v1.Batch.BatchClient;

namespace Nitric.Sdk.Job
{
    /// <summary>
    /// A reference to a job.
    /// </summary>
    public class Job<T>
    {
        internal readonly GrpcClient Client;

        /// <summary>
        /// The name of the job.
        /// </summary>
        public string Name { get; private set; }

        internal Job(string name, GrpcClient client = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Client = client;
            this.Name = name;
        }

        /// <summary>
        /// Submit a job to the batch service
        /// </summary>
        /// <param name="data">Data to submit to the job</param>
        public void Submit(T data)
        {
            var request = new JobSubmitRequest
            {
                JobName = this.Name,
                Data = new JobData
                {
                    Struct = Struct.FromJsonSerializable(data),
                },
            };

            this.Client.SubmitJob(request);
        }

        /// <summary>
        /// Submit a job to the batch service asynchronously
        /// </summary>
        /// <param name="data">Data to submit to the job</param>
        public async void SubmitAsync(T data)
        {
            var request = new JobSubmitRequest
            {
                JobName = this.Name,
                Data = new JobData
                {
                    Struct = Struct.FromJsonSerializable(data),
                },
            };

            await this.Client.SubmitJobAsync(request);
        }

        /// <summary>
        /// Return a string representation of the job.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name + "[name=" + Name + "]";
        }
    }
}
