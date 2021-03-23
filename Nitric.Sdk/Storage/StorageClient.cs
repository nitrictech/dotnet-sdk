using Google.Protobuf;
using ProtoClient = Nitric.Proto.Storage.v1.Storage.StorageClient;
using Nitric.Proto.Storage.v1;
using Nitric.Api.Common;
//using CommonEvent = Nitric.Api.Common.Event;

namespace Nitric.Api.Storage
{
    class StorageClient : AbstractClient
    {
        protected ProtoClient client;

        public StorageClient()
        {
            client = new ProtoClient(GetChannel());
        }
        public void Write(string bucketName, string key, ByteString body)
        {
            var request = new StorageWriteRequest {
                BucketName = bucketName,
                Key = key,
                Body = body
            };
            var response = client.Write(request);
        }
        public byte[] Read(string bucketName, string key)
        {
            var request = new StorageReadRequest {
                BucketName = bucketName,
                Key = key
            };
            var response = client.Read(request);
            return response.ToByteArray();
        }
    }
}