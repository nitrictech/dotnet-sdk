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

using Moq;
using Examples;
using Nitric.Proto.Document.v1;
using Nitric.Proto.Event.v1;
using Nitric.Proto.Queue.v1;
using Nitric.Proto.Secret.v1;
using Nitric.Proto.Storage.v1;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Xunit;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using System;
using Nitric.Sdk.Common.Util;
using Environment = System.Environment;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Nitric.Test.Examples
{
    [Collection("Examples")]
    public class DocumentsExamplesTest : IDisposable
    {
        private readonly Server server;
        private readonly Mock<DocumentService.DocumentServiceBase> mockServer;

        public DocumentsExamplesTest()
        {
            var value = new Value
            {
                StringValue = "document"
            };

            var content = new Struct();
            content.Fields.Add("test", value);

            var document = new Document
            {
                Content = content,
            };
            var documentGetResponse = new DocumentGetResponse
            {
                Document = document,
            };

            //Setup a mock server for the snippets to hit
            mockServer = new Mock<DocumentService.DocumentServiceBase>();
            mockServer.Setup(e => e.Delete(It.IsAny<DocumentDeleteRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new DocumentDeleteResponse())
                .Verifiable();
            mockServer.Setup(e => e.Get(It.IsAny<DocumentGetRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(documentGetResponse)
                .Verifiable();
            mockServer.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new DocumentSetResponse())
                .Verifiable();
            mockServer.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new DocumentQueryResponse())
                .Verifiable();
            this.server = new Server
            {
                Services = { DocumentService.BindService(mockServer.Object) },
                Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        [Fact]
        public void TestDocumentExamples()
        {
            //Call functions
            DeleteExample.DeleteFile();
            GetExample.GetFile();
            SetExample.SetFile();
            PagedResultsExample.PagedResults();
            QueryExample.Query();
            QueryFilterExample.QueryFilter();
            QueryLimitsExample.QueryLimits();
            SubDocQueryExample.QueryDocCol();
        }

        public void Dispose()
        {
            //Verify all the functions were called
            mockServer.Verify(e => e.Delete(It.IsAny<DocumentDeleteRequest>(), It.IsAny<ServerCallContext>()),
                Times.Once);
            mockServer.Verify(e => e.Get(It.IsAny<DocumentGetRequest>(), It.IsAny<ServerCallContext>()), Times.Once);
            mockServer.Verify(e => e.Set(It.IsAny<DocumentSetRequest>(), It.IsAny<ServerCallContext>()), Times.Once);
            mockServer.Verify(e => e.Query(It.IsAny<DocumentQueryRequest>(), It.IsAny<ServerCallContext>()),
                Times.Exactly(6));

            this.server.KillAsync().Wait();
        }
    }

    [Collection("Examples")]
    public class EventsExamplesTest : IDisposable
    {
        private Server server;
        private Mock<EventService.EventServiceBase> mockServer;

        public EventsExamplesTest()
        {
            //Setup a mock server for the snippets to hit
            Environment.SetEnvironmentVariable("SERVICE_ADDRESS", "127.0.0.1:50052");
            mockServer = new Mock<EventService.EventServiceBase>();
            mockServer.Setup(e => e.Publish(It.IsAny<EventPublishRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new EventPublishResponse())
                .Verifiable();
            this.server = new Server
            {
                Services = { EventService.BindService(mockServer.Object) },
                Ports = { new ServerPort("localhost", 50052, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        [Fact]
        public void TestEventsExample()
        {
            //Call functions
            EventIdsExample.EventIdsTopic();
            PublishExample.PublishTopic();

            //Verify all the functions were called
            mockServer.Verify(e => e.Publish(It.IsAny<EventPublishRequest>(), It.IsAny<ServerCallContext>()),
                Times.Exactly(2));
        }

        public void Dispose()
        {
            //Verify all the functions were called
            mockServer.Verify(e => e.Publish(It.IsAny<EventPublishRequest>(), It.IsAny<ServerCallContext>()),
                Times.Exactly(2));

            this.server.KillAsync().Wait();
        }
    }

    [Collection("Examples")]
    public class QueuesExamplesTest : IDisposable
    {
        private readonly Server server;
        private readonly Mock<QueueService.QueueServiceBase> mockServer = null;

        public QueuesExamplesTest()
        {
            var taskToReturn = new NitricTask
            {
                Id = "32",
                LeaseId = "1",
                Payload = Utils.ObjToStruct(new Dictionary<string, string>()),
                PayloadType = "Dictionary"
            };

            var tasks = new RepeatedField<NitricTask> { taskToReturn };

            var queueReceiveResponse = new QueueReceiveResponse();
            queueReceiveResponse.Tasks.AddRange(tasks);

            //Setup a mock server for the snippets to hit
            mockServer = new Mock<QueueService.QueueServiceBase>();
            mockServer.Setup(e => e.Send(It.IsAny<QueueSendRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new QueueSendResponse())
                .Verifiable();
            mockServer.Setup(e => e.Receive(It.IsAny<QueueReceiveRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(queueReceiveResponse)
                .Verifiable();
            mockServer.Setup(e => e.Complete(It.IsAny<QueueCompleteRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new QueueCompleteResponse())
                .Verifiable();
            Environment.SetEnvironmentVariable("SERVICE_ADDRESS", "127.0.0.1:50053");
            this.server = new Server
            {
                Services = { QueueService.BindService(mockServer.Object) },
                Ports = { new ServerPort("localhost", 50053, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        [Fact]
        public void TestQueuesExample()
        {
            //Call functions
            ReceiveExample.ReceiveTask();
            SendExample.SendTask();
        }

        public void Dispose()
        {
            //Verify all the functions were called
            mockServer.Verify(e => e.Send(It.IsAny<QueueSendRequest>(), It.IsAny<ServerCallContext>()), Times.Once);
            mockServer.Verify(e => e.Receive(It.IsAny<QueueReceiveRequest>(), It.IsAny<ServerCallContext>()),
                Times.Once);
            mockServer.Verify(e => e.Complete(It.IsAny<QueueCompleteRequest>(), It.IsAny<ServerCallContext>()),
                Times.Once);

            this.server.KillAsync().Wait();
        }
    }

    [Collection("Examples")]
    public class SecretsExamplesTest : IDisposable
    {
        Server server = null;
        Mock<SecretService.SecretServiceBase> mockServer = null;

        public SecretsExamplesTest()
        {
            var secretPutResponse = new SecretPutResponse
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret
                    {
                        Name = "test-secret",
                    },
                    Version = "test-version",
                }
            };
            var secretAccessResponse = new SecretAccessResponse
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret
                    {
                        Name = "test-secret",
                    },
                    Version = "test-version",
                }
            };
            Environment.SetEnvironmentVariable("SERVICE_ADDRESS", "127.0.0.1:50054");
            //Setup a mock server for the snippets to hit
            mockServer = new Mock<SecretService.SecretServiceBase>();
            mockServer.Setup(e => e.Access(It.IsAny<SecretAccessRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(secretAccessResponse)
                .Verifiable();
            mockServer.Setup(e => e.Put(It.IsAny<SecretPutRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(secretPutResponse)
                .Verifiable();
            this.server = new Server
            {
                Services = { SecretService.BindService(mockServer.Object) },
                Ports = { new ServerPort("localhost", 50054, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        [Fact]
        public void TestSecretsExample()
        {
            //Call functions
            AccessExample.AccessSecret();
            PutExample.PutSecret();
        }

        public void Dispose()
        {
            //Verify all the functions were called
            mockServer.Verify(e => e.Access(It.IsAny<SecretAccessRequest>(), It.IsAny<ServerCallContext>()),
                Times.Once);
            mockServer.Verify(e => e.Put(It.IsAny<SecretPutRequest>(), It.IsAny<ServerCallContext>()), Times.Once);

            this.server.KillAsync().Wait();
        }
    }

    [Collection("Examples")]
    public class StorageExamplesTest : IDisposable
    {
        Server server = null;
        Mock<StorageService.StorageServiceBase> mockServer = null;

        public StorageExamplesTest()
        {
            //Setup a mock server for the snippets to hit
            mockServer = new Mock<StorageService.StorageServiceBase>();
            mockServer.Setup(e => e.Write(It.IsAny<StorageWriteRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new StorageWriteResponse())
                .Verifiable();
            mockServer.Setup(e => e.Read(It.IsAny<StorageReadRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new StorageReadResponse())
                .Verifiable();
            mockServer.Setup(e => e.Delete(It.IsAny<StorageDeleteRequest>(), It.IsAny<ServerCallContext>()))
                .ReturnsAsync(new StorageDeleteResponse())
                .Verifiable();
            Environment.SetEnvironmentVariable("SERVICE_ADDRESS", "127.0.0.1:50055");
            this.server = new Server
            {
                Services = { StorageService.BindService(mockServer.Object) },
                Ports = { new ServerPort("localhost", 50055, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        [Fact]
        public void TestStorageExamples()
        {
            //Call functions
            DeleteExamples.DeleteFile();
            ReadExample.ReadFile();
            WriteExample.WriteFile();
        }

        public void Dispose()
        {
            //Verify all the functions were called
            mockServer.Verify(e => e.Write(It.IsAny<StorageWriteRequest>(), It.IsAny<ServerCallContext>()), Times.Once);
            mockServer.Verify(e => e.Read(It.IsAny<StorageReadRequest>(), It.IsAny<ServerCallContext>()), Times.Once);
            mockServer.Verify(e => e.Delete(It.IsAny<StorageDeleteRequest>(), It.IsAny<ServerCallContext>()),
                Times.Once);

            this.server.KillAsync().Wait();
        }
    }
}
