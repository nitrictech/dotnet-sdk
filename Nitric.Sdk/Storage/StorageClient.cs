using System;
using Google.Protobuf;
using ProtoClient = Nitric.Proto.Storage.v1.Storage.StorageClient;
using Nitric.Proto.Storage.v1;
using Nitric.Api.Common;

namespace Nitric.Api.Storage
{
    public class StorageClient : AbstractClient
    {
        public readonly string BucketName;
        protected ProtoClient client;

        private StorageClient(string bucketName, ProtoClient client)
        {
            this.BucketName = bucketName;
            this.client = (client == null) ? new ProtoClient(GetChannel()) : client;
        }
        public void Write(string key, byte[] body)
        {
            var request = new StorageWriteRequest {
                BucketName = BucketName,
                Key = key,
                Body = ByteString.CopyFrom(body)
            };
            client.Write(request);
        }
        public byte[] Read(string key)
        {
            var request = new StorageReadRequest {
                BucketName = BucketName,
                Key = key
            };
            var response = client.Read(request);
            return response.Body.ToByteArray();
        }
        public void Delete(string key)
        {
            var request = new StorageDeleteRequest
            {
                BucketName = BucketName,
                Key = key
            };
            client.Delete(request);
        }
        public override string ToString()
        {
            return GetType().Name + "[bucket=" + BucketName + "]";
        }
        public class Builder
        {
            private ProtoClient client;
            private string bucketName;
            public Builder()
            {
                this.client = null;
                this.bucketName = null;
            }
            public Builder Client(ProtoClient client)
            {
                this.client = client;
                return this;
            }
            public Builder BucketName(string bucketName)
            {
                this.bucketName = bucketName;
                return this;
            }
            public StorageClient Build()
            {
                if (string.IsNullOrEmpty(this.bucketName))
                {
                    throw new ArgumentNullException("bucketName");
                }
                return new StorageClient(bucketName,client);
            }
        }
    }
}