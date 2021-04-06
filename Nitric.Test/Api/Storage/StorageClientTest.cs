using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nitric.Proto.Storage.v1;
using Moq;
namespace Nitric.Test.Api.Storage
{
    [TestClass]
    public class StorageClientTest
    {
        [TestMethod]
        public void TestBuild()
        {
            var storage = new Nitric.Api.Storage.StorageClient
                .Builder()
                .BucketName("bucket")
                .Build();

            Assert.AreEqual("bucket", storage.BucketName);
            Assert.IsNotNull(storage);

            try
            {
                storage = new Nitric.Api.Storage.StorageClient
                    .Builder()
                    .Build();
                Assert.IsTrue(false);
            } catch(ArgumentNullException ane)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'bucketName')", ane.Message);
            }
        }
        [TestMethod]
        public void TestWrite()
        {
            var request = new StorageWriteRequest
            {
                BucketName = "bucket",
                Key = "key",
                Body = Google.Protobuf.ByteString.CopyFrom(
                    System.Text.Encoding.UTF8.GetBytes("Body"))
            };

            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Write(request, default))
                .Returns(new StorageWriteResponse())
                .Verifiable();

            ec.Verify(t => t.Write(request, default), Times.Once);
        }
        [TestMethod]
        public void TestRead()
        {
            var request = new StorageReadRequest
            {
                BucketName = "bucket",
                Key = "key"
            };
            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Read(request, default))
                .Returns(new StorageReadResponse())
                .Verifiable();

            ec.Verify(t => t.Read(request, default), Times.Once);
        }
        [TestMethod]
        public void TestDelete()
        {
            var request = new StorageDeleteRequest
            {
                BucketName = "bucket",
                Key = "key"
            };
            Mock<Proto.Storage.v1.Storage.StorageClient> ec = new Mock<Proto.Storage.v1.Storage.StorageClient>();
            ec.Setup(e => e.Delete(request, default))
                .Returns(new StorageDeleteResponse())
                .Verifiable();

            ec.Verify(t => t.Delete(request, default), Times.Once);
        }
    }
}
