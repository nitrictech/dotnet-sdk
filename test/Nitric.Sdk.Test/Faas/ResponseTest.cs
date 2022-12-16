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

namespace Nitric.Sdk.Test.Faas
{
    // public class ResponseTest
    // {
    //     [Fact]
    //     public void TestHttpToGrpc()
    //     {
    //         var ctx = new HttpResponseContext()
    //             .SetStatus(200)
    //             .AddHeader("x-nitric-testing", "test");
    //
    //         var response = new Response(
    //           Encoding.UTF8.GetBytes("Hello World"),
    //           ctx
    //         );
    //
    //         var triggerResponse = response.ToGrpcTriggerResponse();
    //
    //         Assert.Equal("Hello World", triggerResponse.Data.ToStringUtf8());
    //         Assert.NotNull(triggerResponse.Http);
    //         Assert.Equal(200, triggerResponse.Http.Status);
    //         Assert.Equal("test", triggerResponse.Http.Headers["x-nitric-testing"].Value[0]);
    //     }
    //
    //     public void TestTopicToGrpc()
    //     {
    //         var ctx = new EventResponse()
    //             .SetSuccess(true);
    //
    //         var response = new Response(
    //           Encoding.UTF8.GetBytes("Hello World"),
    //           ctx
    //         );
    //
    //         var triggerResponse = response.ToGrpcTriggerResponse();
    //
    //         Assert.Equal("Hello World", triggerResponse.Data.ToStringUtf8());
    //         Assert.NotNull(triggerResponse.Topic);
    //         Assert.Equal(true, triggerResponse.Topic.Success);
    //     }
    // }
}
