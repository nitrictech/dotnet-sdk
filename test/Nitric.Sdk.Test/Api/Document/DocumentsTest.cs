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
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using Nitric.Proto.Document.v1;
using Nitric.Sdk.Document;
using Xunit;

namespace Nitric.Sdk.Test.Api.Document
{
    public class DocumentsTest
    {
        [Fact]
        public void TestBuildDocuments()
        {
            var documents = new DocumentsClient();
            Assert.NotNull(documents);
        }

        /*
         * TESTING COLLECTIONS
         */
        [Fact]
        public void TestCollectionsMethodWithName()
        {
            var collection = new DocumentsClient().Collection<Dictionary<string, object>>("test-collection");
            Assert.NotNull(collection);
        }

        [Fact]
        public void TestCollectionsMethodWithoutName()
        {
            var documents = new DocumentsClient();
            Assert.Throws<ArgumentNullException>(
                () => documents.Collection<Dictionary<string, object>>(null));
            Assert.Throws<ArgumentNullException>(
                () => documents.Collection<Dictionary<string, object>>(""));
        }

        [Fact]
        public void TestCollectionSubCollectionWithName()
        {
            var collection = new DocumentsClient().Collection<Dictionary<string, object>>("test-collection");
            var subcollection = collection.Collection("test-subcollection");
            Assert.NotNull(subcollection);
            Assert.Equal(collection, subcollection.ParentKey.Collection);
            Assert.Null(subcollection.ParentKey.Id);
        }

        [Fact]
        public void TestCollectionSubCollectionWithoutName()
        {
            var collection = new DocumentsClient().Collection<Dictionary<string, object>>("test-collection");
            Assert.Throws<ArgumentNullException>(
                () => collection.Collection(null));
            Assert.Throws<ArgumentNullException>(
                () => collection.Collection(""));
        }

        /*
         * TESTING DOCS
         */
        [Fact]
        public void TestBuildDocsWithDocumentId()
        {
            var collection = new DocumentsClient().Collection<Dictionary<string, object>>("test-collection");
            var docs = collection.Doc("test-document");
            Assert.NotNull(docs);
        }

        [Fact]
        public void TestBuildDocWithoutDocumentId()
        {
            var collection = new DocumentsClient().Collection<Dictionary<string, object>>("test-collection");
            Assert.Throws<ArgumentNullException>(
                () => collection.Doc(""));
            Assert.Throws<ArgumentNullException>(
                () => collection.Doc(null));
        }

        [Fact]
        public void TestBuildQuery()
        {
            var collection = new DocumentsClient().Collection<Dictionary<string, object>>("test-collection");
            var query = collection.Query();
            Assert.NotNull(query);
        }

        /*
         * TESTING GET (DOCS)
         */
        [Fact]
        public void TestGetWithDictionary()
        {
            var value = new Value
            {
                StringValue = "document"
            };

            var content = new Struct();
            content.Fields.Add("test", value);

            var document = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documentGetResponse = new DocumentGetResponse
            {
                Document = document,
            };

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Get(It.IsAny<DocumentGetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(documentGetResponse)
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");
            var response = documentRef.Get();

            Assert.Equal("document", response.Content["test"]);
            Assert.Equal(response.Ref, documentRef);

            dc.Verify(
                t => t.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestGetWithSortedDictionary()
        {
            var value = new Value
            {
                StringValue = "document"
            };

            var content = new Struct();
            content.Fields.Add("test", value);

            var document = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documentGetResponse = new DocumentGetResponse
            {
                Document = document,
            };

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Get(It.IsAny<DocumentGetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(documentGetResponse)
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<SortedDictionary<string, object>>("test-collection")
                .Doc("test-document");
            var response = documentRef.Get();
            Assert.Equal("document", response.Content["test"]);
            Assert.Equal(response.Ref, documentRef);

            dc.Verify(
                t => t.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestGetToNonExistentDocument()
        {
            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Get(It.IsAny<DocumentGetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified document does not exist")))
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<SortedDictionary<string, object>>("test-collection")
                .Doc("test-document");
            try
            {
                var response = documentRef.Get();
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified document does not exist\")",
                    ne.Message);
            }

            dc.Verify(
                t => t.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        /*
         * TESTING SET (DOCS)
         */
        [Fact]
        public void TestSetWithSingleEntryDictionary()
        {
            var document = new Dictionary<string, object>();
            document.Add("test", "document");

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(
                t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestSetWithMultipleEntryDictionary()
        {
            var document = new Dictionary<string, object>();
            for (int i = 0; i < 100; i++)
            {
                document.Add(i.ToString(), "document");
            }

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(
                t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestSetWithEmptyDictionary()
        {
            var document = new Dictionary<string, object>();

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(
                t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestSetWithNullDictionaryThrows()
        {
            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            Assert.Throws<ArgumentNullException>(
                () => documentRef.Set(null));

            dc.Verify(
                t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public void TestSetWithSortedDictionary()
        {
            var document = new SortedDictionary<string, object>();
            document.Add("test", "document");

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<SortedDictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(
                t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestSetToDocumentWithoutPermissions()
        {
            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.PermissionDenied,
                    "You do not have permission to modify this document")))
                .Verifiable();

            var document = new SortedDictionary<string, object>();
            document.Add("test", "document");

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<SortedDictionary<string, object>>("test-collection")
                .Doc("test-document");
            try
            {
                documentRef.Set(document);
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal(
                    "Status(StatusCode=\"PermissionDenied\", Detail=\"You do not have permission to modify this document\")",
                    ne.Message);
            }

            dc.Verify(
                t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        /*
         * TESTING DELETE (DOCS)
         */
        [Fact]
        public void TestDelete()
        {
            var documentSetResponse = new DocumentSetResponse();

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Delete(It.IsAny<DocumentDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentDeleteResponse())
                .Verifiable();

            var documentRef = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Delete();

            dc.Verify(
                t => t.Delete(It.IsAny<DocumentDeleteRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        /*
         * TESTING DOC COLLECTION (DOCS)
         */
        [Fact]
        public void TestDocCollectionWithName()
        {
            var docCollection = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document")
                .Collection("test-collection-2");

            Assert.NotNull(docCollection);
        }

        [Fact]
        public void TestDocCollectionWithoutName()
        {
            var document = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            Assert.Throws<ArgumentNullException>(
                () => document.Collection(null));
            Assert.Throws<ArgumentNullException>(
                () => document.Collection(""));
        }

        [Fact]
        public void TestMultipleDocCollectionsThrows()
        {
            Assert.Throws<NotSupportedException>(
                () => new DocumentsClient()
                    .Collection<Dictionary<string, object>>("test-collection")
                    .Doc("test-document")
                    .Collection("test-collection-2")
                    .Doc("test-document-2")
                    .Collection("this-collection-throws-error")
            );
        }

        /*
         * TEST COLLECTION GROUP
         */
        [Fact]
        public void TestCollectionGroupBuildWithName()
        {
            var collection = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection");
            var collectionGroup = collection.Collection("test-subcollection");
            Assert.NotNull(collectionGroup);
            Assert.Equal("test-subcollection", collectionGroup.Name);
            Assert.Null(collectionGroup.ParentKey.Id);
        }

        [Fact]
        public void TestCollectionGroupBuildWithoutName()
        {
            var collection = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection");

            Assert.Throws<ArgumentNullException>(
                () => collection.Collection(null));
            Assert.Throws<ArgumentNullException>(
                () => collection.Collection(""));
        }

        /*
         * TEST QUERY
         */
        [Fact]
        public void TestWhereExpression()
        {
            var query = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query
                    .Where("first_name", "==", "john")
                    .Where("last_name", ">=", "smith")
                    .Where("postcode", "<=", "1234")
                    .Where("address", "startsWith", "B")
                    .Where("id", ">", "0")
                    .Where("id", "<", "5");
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }

        [Fact]
        public void TestChangingLimit()
        {
            var query = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.Limit(100);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }

        [Fact]
        public void TestHandlingNegativeLimits()
        {
            var query = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.Limit(-100);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }

        [Fact]
        public void TestChangingPagingToken()
        {
            var query = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("test", "entry");

            try
            {
                query.PagingFrom(pagingToken);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }

        [Fact]
        public void TestChangingWithNullPagingToken()
        {
            var query = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.PagingFrom(null);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }

        [Fact]
        public void TestChangingWithEmptyPagingToken()
        {
            var query = new DocumentsClient()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.PagingFrom(new Dictionary<string, string>());
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }

        [Fact]
        public void TestFetch()
        {
            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("page-from", "line 10");

            var content = new Struct();
            content.Fields.Add("test", Value.ForString("document"));
            var testDocument = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documents = new Google.Protobuf.Collections.RepeatedField<Proto.Document.v1.Document>();
            documents.Add(testDocument);

            var queryResponse = new DocumentQueryResponse();
            queryResponse.Documents.Add(documents);
            queryResponse.PagingToken.Add(pagingToken);

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query.Fetch();
                Assert.Single(response.QueryData);
                Assert.Equal("document", response.QueryData[0].Content["test"]);
                Assert.Equal(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.False(false);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestFetchConformsToLimitOfOne()
        {
            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("page-from", "line 10");

            var content = new Struct();
            content.Fields.Add("test", Value.ForString("document"));
            var testDocument = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documents = new Google.Protobuf.Collections.RepeatedField<Proto.Document.v1.Document>();
            documents.Add(testDocument);

            var queryResponse = new DocumentQueryResponse();
            queryResponse.Documents.Add(documents);
            queryResponse.PagingToken.Add(pagingToken);

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query
                    .Limit(1)
                    .Fetch();
                Assert.Single(response.QueryData);
                Assert.Equal("document", response.QueryData[0].Content["test"]);
                Assert.Equal(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.False(false);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestFetchWithExpressions()
        {
            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("page-from", "line 10");

            var content = new Struct();
            content.Fields.Add("john", Value.ForString("smith"));
            var testDocument = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documents = new Google.Protobuf.Collections.RepeatedField<Proto.Document.v1.Document>();
            documents.Add(testDocument);

            var queryResponse = new DocumentQueryResponse();
            queryResponse.Documents.Add(documents);
            queryResponse.PagingToken.Add(pagingToken);

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query
                    .Where("first_name", "==", "john")
                    .Where("last_name", "==", "smith")
                    .Fetch();
                Assert.Single(response.QueryData);
                Assert.Equal("document", response.QueryData[0].Content["test"]);
                Assert.Equal(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.False(false);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestFetchWithPagingToken()
        {
            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("page-from", "line 10");

            var updatedPagingToken = new Dictionary<string, string>();
            updatedPagingToken.Add("page-from", "line 11");

            var content = new Struct();
            content.Fields.Add("test", Value.ForString("document"));
            var testDocument = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documents = new Google.Protobuf.Collections.RepeatedField<Proto.Document.v1.Document>();
            documents.Add(testDocument);

            var queryResponse = new DocumentQueryResponse();
            queryResponse.Documents.Add(documents);
            queryResponse.PagingToken.Add(updatedPagingToken);

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query
                    .PagingFrom(pagingToken)
                    .Fetch();
                Assert.Single(response.QueryData);
                Assert.Equal("document", response.QueryData[0].Content["test"]);
                Assert.Equal(response.PagingToken, updatedPagingToken);
            }
            catch (Exception)
            {
                Assert.False(false);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestFetchAll()
        {
            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("page-from", "line 10");

            var content = new Struct();
            content.Fields.Add("0", Value.ForString("john smith"));
            content.Fields.Add("1", Value.ForString("jane doe"));
            var testDocument = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documents = new Google.Protobuf.Collections.RepeatedField<Proto.Document.v1.Document>();
            documents.Add(testDocument);

            var queryResponse = new DocumentQueryResponse();
            queryResponse.Documents.Add(documents);
            queryResponse.PagingToken.Add(pagingToken);

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query.FetchAll();
                Assert.Single(response.QueryData);
                Assert.Equal("john smith", response.QueryData[0].Content["0"]);
                Assert.Equal("jane doe", response.QueryData[1].Content["1"]);
                Assert.Equal(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.False(false);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestFetchNonExistentDocument()
        {
            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified document does not exist")))
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();
            try
            {
                var response = query.Fetch();
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified document does not exist\")",
                    ne.Message);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestFetchFromSubcollection()
        {
            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("page-from", "line 10");

            var content = new Struct();
            content.Fields.Add("test", Value.ForString("document"));
            var testDocument = new Proto.Document.v1.Document
            {
                Content = content,
            };
            var documents = new Google.Protobuf.Collections.RepeatedField<Proto.Document.v1.Document>();
            documents.Add(testDocument);

            var queryResponse = new DocumentQueryResponse();
            queryResponse.Documents.Add(documents);
            queryResponse.PagingToken.Add(pagingToken);

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new DocumentsClient(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Collection("test-subcollection")
                .Query();

            try
            {
                var response = query.Fetch();
                Assert.Single(response.QueryData);
                Assert.Equal("document", response.QueryData[0].Content["test"]);
                Assert.Equal(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.False(false);
            }

            dc.Verify(
                t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
