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
using Nitric.Api.Document;
using Nitric.Proto.Document.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Google.Protobuf.WellKnownTypes;
namespace Nitric.Test.Api.Document
{
    [TestClass]
    public class DocumentsTest
    {
        [TestMethod]
        public void TestBuildDocuments()
        {
            var documents = new Documents();
            Assert.IsNotNull(documents);
        }
        /*
         * TESTING COLLECTIONS
         */
        [TestMethod]
        public void TestCollectionsMethodWithName()
        {
            var collection = new Documents().Collection<Dictionary<string, object>>("test-collection");
            Assert.IsNotNull(collection);
        }
        [TestMethod]
        public void TestCollectionsMethodWithoutName()
        {
            var documents = new Documents();
            Assert.ThrowsException<ArgumentNullException>(
                () => documents.Collection<Dictionary<string, object>>(null));
            Assert.ThrowsException<ArgumentNullException>(
                () => documents.Collection<Dictionary<string, object>>(""));
        }
        /*
         * TESTING DOCS
         */ 
        [TestMethod]
        public void TestBuildDocsWithDocumentId()
        {
            var collection = new Documents().Collection<Dictionary<string, object>>("test-collection");
            var docs = collection.Doc("test-document");
            Assert.IsNotNull(docs);
        }
        [TestMethod]
        public void TestBuildDocWithoutDocumentId()
        {
            var collection = new Documents().Collection<Dictionary<string, object>>("test-collection");
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.Doc(""));
            Assert.ThrowsException<ArgumentNullException>(
                () => collection.Doc(null));
        }
        [TestMethod]
        public void TestBuildQuery()
        {
            var collection = new Documents().Collection<Dictionary<string, object>>("test-collection");
            var query = collection.Query();
            Assert.IsNotNull(query);
        }
        /*
         * TESTING GET (DOCS)
         */
        [TestMethod]
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
            dc.Setup(e => e.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(documentGetResponse)
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");
            var response = documentRef.Get();

            Assert.AreEqual(response.Content["test"], "document");
            Assert.AreEqual(response.Ref, documentRef);

            dc.Verify(t => t.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
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
            dc.Setup(e => e.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(documentGetResponse)
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<SortedDictionary<string, object>>("test-collection")
                .Doc("test-document");
            var response = documentRef.Get();
            Assert.AreEqual(response.Content["test"], "document");
            Assert.AreEqual(response.Ref, documentRef);

            dc.Verify(t => t.Get(It.IsAny<DocumentGetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        /*
         * TESTING SET (DOCS)
         */
        [TestMethod]
        public void TestSetWithSingleEntryDictionary()
        {
            var document = new Dictionary<string, object>();
            document.Add("test", "document");

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSetWithMultipleEntryDictionary()
        {
            var document = new Dictionary<string, object>();
            for (int i = 0; i < 100; i++)
            {
                document.Add(i.ToString(), "document");
            }

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSetWithEmptyDictionary()
        {
            var document = new Dictionary<string, object>();

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSetWithNullDictionaryThrowsException()
        {
            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            Assert.ThrowsException<ArgumentNullException>(
                () => documentRef.Set(null));

            dc.Verify(t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Never);
        }
        [TestMethod]
        public void TestSetWithSortedDictionary()
        {
            var document = new SortedDictionary<string, object>();
            document.Add("test", "document");

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentSetResponse())
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<SortedDictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Set(document);

            dc.Verify(t => t.Set(It.IsAny<DocumentSetRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        /*
         * TESTING DELETE (DOCS)
         */
        [TestMethod]
        public void TestDelete()
        {
            var documentSetResponse = new DocumentSetResponse();

            Mock<DocumentService.DocumentServiceClient> dc = new Mock<DocumentService.DocumentServiceClient>();
            dc.Setup(e => e.Delete(It.IsAny<DocumentDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new DocumentDeleteResponse())
                .Verifiable();

            var documentRef = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            documentRef.Delete();

            dc.Verify(t => t.Delete(It.IsAny<DocumentDeleteRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        /*
         * TESTING DOC COLLECTION (DOCS)
         */
        [TestMethod]
        public void TestDocCollectionWithName()
        {
            var docCollection = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document")
                .Collection("test-collection-2");

            Assert.IsNotNull(docCollection);
        }
        [TestMethod]
        public void TestDocCollectionWithoutName()
        {
            var document = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Doc("test-document");

            Assert.ThrowsException<ArgumentNullException>(
                () => document.Collection(null));
            Assert.ThrowsException<ArgumentNullException>(
                () => document.Collection(""));
        }
        [TestMethod]
        public void TestMultipleDocCollectionsThrowsException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => new Documents()
                    .Collection<Dictionary<string, object>>("test-collection")
                    .Doc("test-document")
                    .Collection("test-collection-2")
                    .Doc("test-document-2")
                    .Collection("this-collection-throws-error")
            );
        }
        /*
         * TEST QUERY
         */
        [TestMethod]
        public void TestWhereExpression()
        {
            var query = new Documents()
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
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
        public void TestChangingLimit()
        {
            var query = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.Limit(100);
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
        public void TestHandlingNegativeLimits()
        {
            var query = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.Limit(-100);
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
        public void TestChangingPagingToken()
        {
            var query = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            var pagingToken = new Dictionary<string, string>();
            pagingToken.Add("test", "entry");

            try
            {
                query.PagingFrom(pagingToken);
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
        public void TestChangingWithNullPagingToken()
        {
            var query = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.PagingFrom(null);
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
        public void TestChangingWithEmptyPagingToken()
        {
            var query = new Documents()
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                query.PagingFrom(new Dictionary<string, string>());
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
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
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query.Fetch();
                Assert.AreEqual(response.QueryData.Count, 1);
                Assert.AreEqual(response.QueryData[0].Content["test"], "document");
                Assert.AreEqual(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            dc.Verify(t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
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
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query
                    .Limit(1)
                    .Fetch();
                Assert.AreEqual(response.QueryData.Count, 1);
                Assert.AreEqual(response.QueryData[0].Content["test"], "document");
                Assert.AreEqual(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            dc.Verify(t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
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
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query
                    .Where("first_name", "==", "john")
                    .Where("last_name", "==", "smith")
                    .Fetch();
                Assert.AreEqual(response.QueryData.Count, 1);
                Assert.AreEqual(response.QueryData[0].Content["test"], "document");
                Assert.AreEqual(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            dc.Verify(t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
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
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query
                    .PagingFrom(pagingToken)
                    .Fetch();
                Assert.AreEqual(response.QueryData.Count, 1);
                Assert.AreEqual(response.QueryData[0].Content["test"], "document");
                Assert.AreEqual(response.PagingToken, updatedPagingToken);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            dc.Verify(t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
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
            dc.Setup(e => e.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(queryResponse)
                .Verifiable();

            var query = new Documents(dc.Object)
                .Collection<Dictionary<string, object>>("test-collection")
                .Query();

            try
            {
                var response = query.FetchAll();
                Assert.AreEqual(response.QueryData.Count, 1);
                Assert.AreEqual(response.QueryData[0].Content["0"], "john smith");
                Assert.AreEqual(response.QueryData[1].Content["1"], "jane doe");
                Assert.AreEqual(response.PagingToken, pagingToken);
            }
            catch (Exception)
            {
                Assert.IsFalse(false);
            }

            dc.Verify(t => t.Query(It.IsAny<DocumentQueryRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
