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

using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using Google.Protobuf;
using Nitric.Sdk.Common.Util;

namespace Nitric.Sdk.Queue
{
    /// <summary>
    /// Represents a task that was unable to be sent to a queue.
    /// </summary>
    public class FailedTask
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The task that failed to be sent.
        /// </summary>
        public Task Task { get; set; }        

        /// <summary>
        /// Return a string representation of the failed task.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name + "[task=" + Task + ", message=" + Message + "]";
        }
    }
}
