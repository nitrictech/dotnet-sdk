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
using Xunit;
using Nitric.Sdk.Queue;

namespace Nitric.Sdk.Test.Queue
{
    public class QueueItemTest
    {
        [Fact]
        public void TestBuildReceivedMessage()
        {
            var payload = new TestProfile { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };
            var queueItem = new ReceivedMessage<TestProfile>
            {
                LeaseId = "1",
                Message = payload,
            };

            Assert.NotNull(queueItem);
            Assert.Equal("1", queueItem.LeaseId);
            Assert.Equal(payload, queueItem.Message);
        }

        [Fact]
        public void TestBuildFailedMessage()
        {
            var payload = new TestProfile { Name = "John Smith", Age = 30, Addresses = new List<string> { "123 street st" } };
            var failedMessage = new FailedMessage<TestProfile>
            {
                Details = "The failed task failed successfully",
                Message = payload,
            };

            Assert.NotNull(failedMessage);
            Assert.Equal("The failed task failed successfully", failedMessage.Details);
            Assert.Equal(payload, failedMessage.Message);
            Assert.Equal("FailedMessage[details=The failed task failed successfully]", failedMessage.ToString());
        }
    }
}
