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
using Nitric.Sdk.Job;
using Nitric.Proto.Batch.v1;
using Nitric.Proto.Resources.v1;
using ResourceType = Nitric.Proto.Resources.v1.ResourceType;
using System.Collections.Generic;
using Action = Nitric.Proto.Resources.v1.Action;
using System.Linq;

namespace Nitric.Sdk.Resource
{
    ///<Summary>
    /// Available permissions for job resources.
    ///</Summary>
    public enum JobPermission
    {
        /// <summary>
        /// Enables submitting jobs to the batch service
        /// </summary>
        Submit,
    }

    public class JobResourceRequirements
    {
        /// <summary>
        /// The amount of CPUs to allocate for this job
        /// </summary>
        public int Cpus { get; private set; }

        /// <summary>
        /// The amount of memory to allocate for this job (in MB)
        /// </summary>
        public int Memory { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Gpus { get; private set; }



        public JobResourceRequirements(int cpus = 1, int memory = 1024, int gpus = 0)
        {
            this.Cpus = cpus;
            this.Memory = memory;
            this.Gpus = gpus;
        }
    }

    public class JobResource<T> : SecureResource<JobPermission>
    {
        internal JobResource(string name) : base(name, ResourceType.Batch)
        {

        }

        internal override BaseResource Register()
        {
            var request = new ResourceDeclareRequest { Id = this.AsProtoResource() };
            BaseResource.client.Declare(request);
            return this;
        }

        protected override IEnumerable<Action> PermissionsToActions(IEnumerable<JobPermission> permissions)
        {
            var actionMap = new Dictionary<JobPermission, List<Action>>
            {
                {
                    JobPermission.Submit,
                    new List<Action> { Action.JobSubmit }
                }
            };

            return permissions.Aggregate((IEnumerable<Action>)new List<Action>(), (acc, x) => acc.Concat(actionMap[x])).Distinct();
        }

        public void Handler(Func<JobContext<T>, JobContext<T>> middlewares, JobResourceRequirements requirements = null)
        {
            requirements ??= new JobResourceRequirements();

            var registrationRequest = new RegistrationRequest
            {
                JobName = this.Name,
                Requirements = new Proto.Batch.v1.JobResourceRequirements
                {
                    Cpus = requirements.Cpus,
                    Memory = requirements.Memory,
                    Gpus = requirements.Gpus,
                }
            };

            var apiWorker = new JobWorker<T>(registrationRequest, middlewares);

            Nitric.RegisterWorker(apiWorker);
        }

        /// <summary>
        /// Request specific access to this job.
        /// </summary>
        /// <param name="permissions">The permissions that the function has to access the job.</param>
        /// <returns>A reference to the job.</returns>
        public Job<T> Allow(JobPermission permission, params JobPermission[] permissions)
        {
            var allPerms = new List<JobPermission> { permission };
            allPerms.AddRange(permissions);

            this.RegisterPolicy(allPerms);

            return BatchClient.Job<T>(this.Name);
        }
    }
}
