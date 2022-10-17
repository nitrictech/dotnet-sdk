using Google.Protobuf;
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Storage
{
    /// <summary>
    /// A reference to a specific file in a bucket.
    /// </summary>
    public class File
    {
        private readonly Storage storage;
        private readonly Bucket bucket;
        private string Key { get; set; }

        internal File(Storage storage, Bucket bucket, string key)
        {
            this.storage = storage;
            this.bucket = bucket;
            this.Key = key;
        }

        /// <summary>
        /// Create or update the contents of the file.
        /// </summary>
        /// <param name="body">The contents to write.</param>
        /// <exception cref="NitricException"></exception>
        public void Write(byte[] body)
        {
            var request = new StorageWriteRequest
            {
                BucketName = bucket.Name,
                Key = this.Key,
                Body = ByteString.CopyFrom(body)
            };
            try
            {
                storage.Client.Write(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Retrieve the contents of a file.
        /// </summary>
        /// <returns>The file contents.</returns>
        /// <exception cref="NitricException"></exception>
        public byte[] Read()
        {
            var request = new StorageReadRequest
            {
                BucketName = bucket.Name,
                Key = this.Key
            };
            try
            {
                var response = storage.Client.Read(request);
                return response.Body.ToByteArray();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Delete the file.
        /// </summary>
        /// <exception cref="NitricException"></exception>
        public void Delete()
        {
            var request = new StorageDeleteRequest
            {
                BucketName = bucket.Name,
                Key = this.Key
            };
            try
            {
                storage.Client.Delete(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Return a string representation of the file. Will not contain the file contents.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name + "[key=" + Key + "\nbucket=" + bucket.Name + "]";
        }
    }
}
