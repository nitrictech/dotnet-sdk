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
using System.Text;
using Grpc.Core;
using Moq;
using Nitric.Proto.Secret.v1;
using Nitric.Sdk.Secret;
using Xunit;


namespace Nitric.Sdk.Test.Api.Secret
{
    public class SecretTest
    {
        [Fact]
        public void TestBuildSecrets()
        {
            var secrets = new SecretsClient();
            Assert.NotNull(secrets);
        }

        [Fact]
        public void TestBuildSecretsWithNullClient()
        {
            var secrets = new SecretsClient(null);
            Assert.NotNull(secrets);
        }

        [Fact]
        public void TestBuildSecretWithName()
        {
            var secret = new SecretsClient().Secret("test-secret");
            Assert.NotNull(secret);
            Assert.Equal("test-secret", secret.Name);
        }

        [Fact]
        public void TestBuildSecretWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SecretsClient().Secret(""));
            Assert.Throws<ArgumentNullException>(
                () => new SecretsClient().Secret(null));
        }

        //Testing Secret Methods
        [Fact]
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
            sc.Setup(e =>
                    e.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var testBytes = Encoding.UTF8.GetBytes("Super secret message");
            var secret = new SecretsClient(sc.Object)
                .Secret("test-secret");
            var response = secret.Put(testBytes);

            Assert.Equal("test-version", response.Id);
            Assert.Equal(secret.Name, response.Secret.Name);

            sc.Verify(
                t => t.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
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
            sc.Setup(e =>
                    e.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var testString = "Super secret message";
            var secret = new SecretsClient(sc.Object)
                .Secret("test-secret");
            var response = secret.Put(testString);

            Assert.Equal("test-version", response.Id);
            Assert.Equal(secret.Name, response.Secret.Name);

            sc.Verify(
                t => t.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestPutEmptySecretBytes()
        {
            var secret = new SecretsClient().Secret("test-secret");
            Assert.Throws<ArgumentNullException>(
                () => secret.Put(new byte[] { }));
        }

        [Fact]
        public void TestPutNullSecretBytes()
        {
            var secret = new SecretsClient().Secret("test-secret");
            Assert.Throws<ArgumentNullException>(
                () => secret.Put((byte[])null));
        }

        [Fact]
        public void TestPutEmptySecretString()
        {
            var secret = new SecretsClient().Secret("test-secret");
            Assert.Throws<ArgumentNullException>(
                () => secret.Put(""));
        }

        [Fact]
        public void TestPutNullSecretString()
        {
            var secret = new SecretsClient().Secret("test-secret");
            Assert.Throws<ArgumentNullException>(
                () => secret.Put((string)null));
        }

        [Fact]
        public void TestPutNonExistentSecret()
        {
            Mock<SecretService.SecretServiceClient> sc = new Mock<SecretService.SecretServiceClient>();
            sc.Setup(e =>
                    e.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "The specified secret does not exist")))
                .Verifiable();

            var testString = "Super secret message";
            var secret = new SecretsClient(sc.Object)
                .Secret("test-secret");
            try
            {
                var response = secret.Put(testString);
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal("Status(StatusCode=\"NotFound\", Detail=\"The specified secret does not exist\")",
                    ne.Message);
            }


            sc.Verify(
                t => t.Put(It.IsAny<SecretPutRequest>(), null, null, It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void TestGetSecretVersion()
        {
            var secret = new SecretsClient().Secret("test-secret");
            var secretVersion = secret.Version("test-version");
            Assert.NotNull(secretVersion);
            Assert.Equal("test-version", secretVersion.Id);
            Assert.Equal(secret, secretVersion.Secret);
        }

        [Fact]
        public void TestGetSecretVersionWithoutName()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SecretsClient().Secret("test-secret").Version(""));
            Assert.Throws<ArgumentNullException>(
                () => new SecretsClient().Secret("test-secret").Version(null));
        }

        [Fact]
        public void TestAccessSecretWithoutPermission()
        {
            Mock<SecretService.SecretServiceClient> sc = new Mock<SecretService.SecretServiceClient>();
            sc.Setup(e => e.Access(It.IsAny<SecretAccessRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Throws(new RpcException(new Status(StatusCode.PermissionDenied,
                    "You do not have permission to access this secret")))
                .Verifiable();

            var secret = new SecretsClient(sc.Object)
                .Secret("test-secret");
            try
            {
                var response = secret.Version("test-secret").Access();
            }
            catch (global::Nitric.Sdk.Common.NitricException ne)
            {
                Assert.Equal(
                    "Status(StatusCode=\"PermissionDenied\", Detail=\"You do not have permission to access this secret\")",
                    ne.Message);
            }


            sc.Verify(
                t => t.Access(It.IsAny<SecretAccessRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestGetLatestSecretVersion()
        {
            var secretVersion = new SecretsClient()
                .Secret("test-secret")
                .Latest();
            Assert.Equal("latest", secretVersion.Id);
        }

        [Fact]
        public void TestSecretToString()
        {
            var secretString = new SecretsClient()
                .Secret("test-secret")
                .ToString();
            Assert.Equal("[name=test-secret]", secretString);
        }

        //Testing Secret Version Methods
        [Fact]
        public void TestAccess()
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
            sc.Setup(e => e.Access(It.IsAny<SecretAccessRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var version = new SecretsClient(sc.Object)
                .Secret("test-secret")
                .Version("test-version");
            var response = version.Access();

            var responseString = Encoding.UTF8.GetString(response.Value);
            Assert.Equal("test-version", response.SecretVersion.Id);
            Assert.Equal("test-secret", response.SecretVersion.Secret.Name);
            Assert.Equal("Super secret message", Encoding.UTF8.GetString(response.Value));
            Assert.Equal("Super secret message", response.ValueText);

            sc.Verify(
                t => t.Access(It.IsAny<SecretAccessRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TestSecretVersionToString()
        {
            var secretVersionString = new SecretsClient()
                .Secret("test-secret")
                .Version("test-version")
                .ToString();
            Assert.Equal("SecretVersion[secret=[name=test-secret], version=test-version]", secretVersionString);
        }

        [Fact]
        public void TestSecretValueToString()
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
            sc.Setup(e => e.Access(It.IsAny<SecretAccessRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(secretPutResponse)
                .Verifiable();

            var version = new SecretsClient(sc.Object)
                .Secret("test-secret")
                .Version("test-version");

            var response = version.Access();

            Assert.Equal(
                "SecretValue[secretVersion=SecretVersion[secret=[name=test-secret], version=test-version], value.length=20]",
                response.ToString());

            sc.Verify(
                t => t.Access(It.IsAny<SecretAccessRequest>(), null, null,
                    It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
