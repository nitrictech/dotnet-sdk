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
using Google.Protobuf;
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Common;
using Nitric.Sdk.Function;
using ProtoFile = Nitric.Proto.Storage.v1.File;

namespace Nitric.Sdk.Storage
{
    ///<Summary>
    /// Available operations for signing a url.
    ///</Summary>
    public enum FileMode
    {
        /// <summary>
        /// Download a file from a bucket.
        /// </summary>
        Read,
        /// <summary>
        /// Upload a file to a bucket.
        /// </summary>
        Write,
    }

    /// <summary>
    /// A reference to a specific file in a bucket.
    /// </summary>
    public class File
    {
        private readonly Storage storage;
        private readonly Bucket bucket;
        public string Name { get; private set; }


        internal File(Storage storage, Bucket bucket, string key)
        {
            this.storage = storage;
            this.bucket = bucket;
            this.Name = key;
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
                Key = this.Name,
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
        /// Create or update the contents of the file.
        /// </summary>
        /// <param name="body">The contents to write.</param>
        /// <exception cref="NitricException"></exception>
        public void Write(string body)
        {
            var request = new StorageWriteRequest
            {
                BucketName = bucket.Name,
                Key = this.Name,
                Body = ByteString.CopyFromUtf8(body)
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
                Key = this.Name
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
                Key = this.Name
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
        /// Create a presigned URL for reading or writing for the given file reference.
        /// </summary>
        /// <param name="mode">The mode the URL will access the file with. E.g. reading or writing.</param>
        /// <param name="expiry">How long the URL should be valid for in seconds.</param>
        /// <returns>The signed URL for reading or writing</returns>
        internal string PreSignUrl(FileMode mode, int expiry)
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = this.bucket.Name,
                Key = this.Name,
                Operation = mode == FileMode.Read ? StoragePreSignUrlRequest.Types.Operation.Read : StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = expiry < 0 ? 0 : (uint)expiry,

            };

            try
            {
                var resp = storage.Client.PreSignUrl(request);
                return resp.Url;
            } catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Create a presigned URL for reading a given file reference.
        /// </summary>
        /// <param name="expiry">How long the URL should be valid for in seconds. Defaults to 600 seconds (10 minutes).</param>
        /// <returns>The signed URL for reading.</returns>
        public string GetDownloadUrl(int expiry = 600)
        {
            return this.PreSignUrl(FileMode.Write, expiry);
        }

        /// <summary>
        /// Create a presigned URL for writing to a given file reference.
        /// </summary>
        /// <param name="expiry">How long the URL should be valid for in seconds. Defaults to 600 seconds (10 minutes).</param>
        /// <returns>The signed URL for writing.</returns>
        public string GetUploadUrl(int expiry = 600)
        {
            return this.PreSignUrl(FileMode.Read, expiry);
        }

        /// <summary>
        /// Return a string representation of the file. Will not contain the file contents.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name + "[name=" + Name + "\nbucket=" + bucket.Name + "]";
        }
    }
}
