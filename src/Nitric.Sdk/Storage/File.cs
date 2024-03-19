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
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Nitric.Proto.Storage.v1;
using Nitric.Sdk.Common;

namespace Nitric.Sdk.Storage
{
    ///<Summary>
    /// Available operations for signing a url.
    ///</Summary>
    public enum SignedMode
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
        private readonly Bucket Bucket;
        public string Name { get; private set; }


        internal File(Bucket bucket, string key)
        {
            this.Bucket = bucket;
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
                BucketName = Bucket.Name,
                Key = this.Name,
                Body = ByteString.CopyFrom(body)
            };
            try
            {
                this.Bucket.Storage.Client.Write(request);
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
                BucketName = Bucket.Name,
                Key = this.Name,
                Body = ByteString.CopyFromUtf8(body)
            };
            try
            {
                this.Bucket.Storage.Client.Write(request);
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
        public async Task WriteAsync(byte[] body)
        {
            var request = new StorageWriteRequest
            {
                BucketName = Bucket.Name,
                Key = this.Name,
                Body = ByteString.CopyFrom(body)
            };
            try
            {
                await this.Bucket.Storage.Client.WriteAsync(request);
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
        public async Task WriteAsync(string body)
        {
            var request = new StorageWriteRequest
            {
                BucketName = Bucket.Name,
                Key = this.Name,
                Body = ByteString.CopyFromUtf8(body)
            };
            try
            {
                await this.Bucket.Storage.Client.WriteAsync(request);
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
                BucketName = Bucket.Name,
                Key = this.Name
            };
            try
            {
                var response = this.Bucket.Storage.Client.Read(request);
                return response.Body.ToByteArray();
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
        public async Task<byte[]> ReadAsync()
        {
            var request = new StorageReadRequest
            {
                BucketName = Bucket.Name,
                Key = this.Name
            };
            try
            {
                var response = await this.Bucket.Storage.Client.ReadAsync(request);
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
                BucketName = Bucket.Name,
                Key = this.Name
            };
            try
            {
                this.Bucket.Storage.Client.Delete(request);
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
        public async Task DeleteAsync()
        {
            var request = new StorageDeleteRequest
            {
                BucketName = Bucket.Name,
                Key = this.Name
            };
            try
            {
                await this.Bucket.Storage.Client.DeleteAsync(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Create a presigned URL for writing to a given file reference.
        /// </summary>
        /// <param name="expiry">How long the URL should be valid for in seconds. Defaults to 600 seconds (10 minutes).</param>
        /// <returns>The signed URL for writing.</returns>
        public string GetUploadUrl(int expiry = 600)
        {
            return this.PreSignUrl(SignedMode.Write, expiry);
        }

        /// <summary>
        /// Create a presigned URL for reading a given file reference.
        /// </summary>
        /// <param name="expiry">How long the URL should be valid for in seconds. Defaults to 600 seconds (10 minutes).</param>
        /// <returns>The signed URL for reading.</returns>
        public string GetDownloadUrl(int expiry = 600)
        {
            return this.PreSignUrl(SignedMode.Read, expiry);
        }

        /// <summary>
        /// Create a presigned URL for reading or writing for the given file reference.
        /// </summary>
        /// <param name="mode">The mode the URL will access the file with. E.g. reading or writing.</param>
        /// <param name="expiry">How long the URL should be valid for in seconds (max of 604800).</param>
        /// <returns>The signed URL for reading or writing</returns>
        internal string PreSignUrl(SignedMode mode, int expiry)
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = this.Bucket.Name,
                Key = this.Name,
                Operation = mode == SignedMode.Read ? StoragePreSignUrlRequest.Types.Operation.Read : StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = new Duration
                {
                    Seconds = Math.Clamp(expiry, 0, 604800),
                }
            };

            try
            {
                var resp = this.Bucket.Storage.Client.PreSignUrl(request);
                return resp.Url;
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }

        /// <summary>
        /// Create a presigned URL for reading a given file reference.
        /// </summary>
        /// <param name="expiry">How long the URL should be valid for in seconds. Defaults to 600 seconds (10 minutes).</param>
        /// <returns>The signed URL for reading.</returns>
        public async Task<string> GetDownloadUrlAsync(int expiry = 600)
        {
            return await this.PreSignUrlAsync(SignedMode.Read, expiry);
        }


        /// <summary>
        /// Create a presigned URL for writing to a given file reference.
        /// </summary>
        /// <param name="expiry">How long the URL should be valid for in seconds. Defaults to 600 seconds (10 minutes).</param>
        /// <returns>The signed URL for writing.</returns>
        public async Task<string> GetUploadUrlAsync(int expiry = 600)
        {
            return await this.PreSignUrlAsync(SignedMode.Write, expiry);
        }

        /// <summary>
        /// Create a presigned URL for reading or writing for the given file reference.
        /// </summary>
        /// <param name="mode">The mode the URL will access the file with. E.g. reading or writing.</param>
        /// <param name="expiry">How long the URL should be valid for in seconds (max of 604800).</param>
        /// <returns>The signed URL for reading or writing</returns>
        internal async Task<string> PreSignUrlAsync(SignedMode mode, int expiry)
        {
            var request = new StoragePreSignUrlRequest
            {
                BucketName = this.Bucket.Name,
                Key = this.Name,
                Operation = mode == SignedMode.Read ? StoragePreSignUrlRequest.Types.Operation.Read : StoragePreSignUrlRequest.Types.Operation.Write,
                Expiry = new Duration
                {
                    Seconds = Math.Clamp(expiry, 0, 604800),
                }
            };

            try
            {
                var resp = await this.Bucket.Storage.Client.PreSignUrlAsync(request);
                return resp.Url;
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
            return GetType().Name + "[name=" + Name + "\nbucket=" + Bucket.Name + "]";
        }
    }
}
