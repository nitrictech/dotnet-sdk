using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nitric.Api.Secret;
using Nitric.Proto.Secret.v1;
namespace Nitric.Test.Api.Secret
{
    [TestClass]
    public class SecretTest
    {
        [TestMethod]
        public void TestBuildSecrets()
        {
            var secrets = new Secrets();
            Assert.IsNotNull(secrets);
        }
        [TestMethod]
        public void TestBuildSecretsWithNullClient()
        {
            var secrets = new Secrets(null);
            Assert.IsNotNull(secrets);
        }
        [TestMethod]
        public void TestBuildSecretWithName()
        {
            var secret = new Secrets().Secret("test-secret");
            Assert.IsNotNull(secret);
            Assert.AreEqual("test-secret", secret.Name);
        }
        [TestMethod]
        public void TestBuildSecretWithoutName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Secrets().Secret(""));
            Assert.ThrowsException<ArgumentNullException>(
                () => new Secrets().Secret(null));
        }
        //Testing Secret Methods
        [TestMethod]
        public void TestPutSecretBytes()
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
            Mock<SecretService.SecretServiceClient> sc = new Mock<SecretService.SecretServiceClient>();
            sc.Setup(e => e.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var testBytes = Encoding.UTF8.GetBytes("Super secret message");
            var secret = new Secrets(sc.Object)
                .Secret("test-secret");
            var response = secret.Put(testBytes);

            Assert.AreEqual("test-version", response.Version);
            Assert.AreEqual(secret.Name, response.Secret.Name);

            sc.Verify(t => t.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestPutSecretString()
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
            Mock<SecretService.SecretServiceClient> sc = new Mock<SecretService.SecretServiceClient>();
            sc.Setup(e => e.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var testString = "Super secret message";
            var secret = new Secrets(sc.Object)
                .Secret("test-secret");
            var response = secret.Put(testString);

            Assert.AreEqual("test-version", response.Version);
            Assert.AreEqual(secret.Name, response.Secret.Name);

            sc.Verify(t => t.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestPutEmptySecretBytes()
        {
            var secret = new Secrets().Secret("test-secret");
            Assert.ThrowsException<ArgumentNullException>(
                () => secret.Put(new byte[] { }));
        }
        [TestMethod]
        public void TestPutNullSecretBytes()
        {
            var secret = new Secrets().Secret("test-secret");
            Assert.ThrowsException<ArgumentNullException>(
                () => secret.Put((byte[])null));
        }
        [TestMethod]
        public void TestPutEmptySecretString()
        {
            var secret = new Secrets().Secret("test-secret");
            Assert.ThrowsException<ArgumentNullException>(
                () => secret.Put(""));
        }
        [TestMethod]
        public void TestPutNullSecretString()
        {
            var secret = new Secrets().Secret("test-secret");
            Assert.ThrowsException<ArgumentNullException>(
                () => secret.Put((string)null));
        }
        [TestMethod]
        public void TestGetSecretVersion()
        {
            var secret = new Secrets().Secret("test-secret");
            var secretVersion = secret.Version("test-version");
            Assert.IsNotNull(secretVersion);
            Assert.AreEqual("test-version", secretVersion.Version);
            Assert.AreEqual(secret, secretVersion.Secret);
        }
        [TestMethod]
        public void TestGetSecretVersionWithoutName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Secrets().Secret("test-secret").Version(""));
            Assert.ThrowsException<ArgumentNullException>(
                () => new Secrets().Secret("test-secret").Version(null));
        }
        [TestMethod]
        public void TestGetLatestSecretVersion()
        {
            var secretVersion = new Secrets()
                .Secret("test-secret")
                .Latest();
            Assert.AreEqual("latest", secretVersion.Version);
        }
        [TestMethod]
        public void TestSecretToString()
        {
            var secretString = new Secrets()
                .Secret("test-secret")
                .ToString();
            Assert.AreEqual("[name=test-secret]", secretString);
        }
        //Testing Secret Version Methods
        [TestMethod]
        public void TestGetValueBytes()
        {
            var secretPutResponse = new SecretAccessResponse
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret
                    {
                        Name = "test-secret",
                    },
                    Version = "test-version",
                },
                Value = Google.Protobuf.ByteString.CopyFromUtf8("Super secret message"),
            };
            Mock<SecretService.SecretServiceClient> sc = new Mock<SecretService.SecretServiceClient>();
            sc.Setup(e => e.Access(It.IsAny<SecretAccessRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var secret = new Secrets(sc.Object)
                .Secret("test-secret");
            var response = secret.Version("test-version").Access();
            var responseString = Encoding.UTF8.GetString(response);
            Assert.AreEqual("Super secret message", responseString);

            sc.Verify(t => t.Access(It.IsAny<SecretAccessRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestGetValueString()
        {
            var secretPutResponse = new SecretAccessResponse
            {
                SecretVersion = new Proto.Secret.v1.SecretVersion
                {
                    Secret = new Proto.Secret.v1.Secret
                    {
                        Name = "test-secret",
                    },
                    Version = "test-version",
                },
                Value = Google.Protobuf.ByteString.CopyFromUtf8("Super secret message"),
            };
            Mock<SecretService.SecretServiceClient> sc = new Mock<SecretService.SecretServiceClient>();
            sc.Setup(e => e.Access(It.IsAny<SecretAccessRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var secret = new Secrets(sc.Object)
                .Secret("test-secret");
            var response = secret.Version("test-version").AccessText();

            Assert.AreEqual("Super secret message", response);

            sc.Verify(t => t.Access(It.IsAny<SecretAccessRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public void TestSecretVersionToString()
        {
            var secretVersionString = new Secrets()
                .Secret("test-secret")
                .Version("test-version")
                .ToString();
            Assert.AreEqual("SecretVersion[secret=[name=test-secret], version=test-version]", secretVersionString);
        }
    }
}
