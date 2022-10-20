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
    // public class TriggerTest
    // {
    //     [Fact]
    //     public void TestFromGrpcTriggerRequestHttp()
    //     {
    //         var triggerRequest = new TriggerRequest();
    //         triggerRequest.Data = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes("Hello World"));
    //         triggerRequest.Http = new HttpTriggerContext();
    //         triggerRequest.Http.Method = "GET";
    //         var testHeader = new HeaderValue();
    //         testHeader.Value.Add("test");
    //         triggerRequest.Http.Headers.Add("x-nitric-test", testHeader);
    //         triggerRequest.MimeType = "text/plain";
    //         triggerRequest.Http.Path = "/test/";
    //
    //         var trigger = Trigger.FromGrpcTriggerRequest(triggerRequest);
    //
    //
    //         Assert.True(trigger.Context.IsHttp());
    //         Assert.Equal("Hello World", Encoding.UTF8.GetString(trigger.Data));
    //         Assert.Equal("text/plain", trigger.MimeType);
    //         Assert.Equal("GET", trigger.Context.AsHttp().Method);
    //         Assert.Equal("/test/", trigger.Context.AsHttp().Path);
    //         Assert.Equal("test", trigger.Context.AsHttp().Headers["x-nitric-test"][0]);
    //     }
    //
    //     [Fact]
    //     public void TestFromGrpcTriggerTopic()
    //     {
    //         var triggerRequest = new TriggerRequest();
    //         triggerRequest.Data = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes("Hello World"));
    //         triggerRequest.MimeType = "text/plain";
    //         triggerRequest.Topic = new Nitric.Proto.Faas.v1.TopicTriggerContext();
    //         triggerRequest.Topic.Topic = "Test";
    //
    //         var trigger = Trigger.FromGrpcTriggerRequest(triggerRequest);
    //
    //         Assert.True(trigger.Context.IsTopic());
    //         Assert.Equal("Hello World", Encoding.UTF8.GetString(trigger.Data));
    //         Assert.Equal("text/plain", trigger.MimeType);
    //         Assert.Equal("Test", trigger.Context.AsTopic().Topic);
    //     }
    // }
}
