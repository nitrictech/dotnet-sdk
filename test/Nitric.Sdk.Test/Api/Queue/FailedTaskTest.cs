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
﻿using System;
using System.Collections.Generic;
using Xunit;
using Google.Protobuf.WellKnownTypes;

namespace Nitric.Test.Api.QueueClient
{
    public class FailedTaskTest
    {
        [Fact]
        public void TestBuild()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("name", "value");
            Struct payloadStruct = Nitric.Api.Common.Util.ObjToStruct(payload);
            var failedTask = Nitric.Api.Queue.FailedTask
                .NewBuilder()
                .Id("1")
                .PayloadType("payload type")
                .Payload(payloadStruct)
                .Message("message")
                .Build();

            Assert.NotNull(failedTask);
            Assert.Equal("1", failedTask.Task.ID);
            Assert.Equal("payload type", failedTask.Task.PayloadType);
            Assert.Equal(payloadStruct, failedTask.Task.Payload);
            Assert.Equal("message", failedTask.Message);
        }
    }
}
