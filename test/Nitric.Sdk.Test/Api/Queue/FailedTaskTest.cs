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

using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Nitric.Sdk.Queue;
using Xunit;

namespace Nitric.Sdk.Test.Api.Queue
{
    public class TestProfile
    {
        public string Name;
        public double Age;
        public List<string> Addresses;
    }

    public class FailedTaskTest
    {
        [Fact]
        public void TestBuild()
        {
            var payload = new TestProfile
                { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st " } };
            var failedTask = new global::Nitric.Sdk.Queue.FailedTask<TestProfile>
            {
                Message = "message",
                Task = new Task<TestProfile>
                {
                    Id = "1",
                    PayloadType = "payload type",
                    Payload = payload
                }
            };

            Assert.NotNull(failedTask);
            Assert.Equal("1", failedTask.Task.Id);
            Assert.Equal("payload type", failedTask.Task.PayloadType);
            Assert.Equal(payload, failedTask.Task.Payload);
            Assert.Equal("message", failedTask.Message);
        }
    }
}
