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
using Google.Protobuf;
using GrpcClient = Nitric.Proto.Storage.v1.StorageService.StorageServiceClient;
using Nitric.Proto.Storage.v1;
using Nitric.Api.Common;

namespace Nitric.Api.Storage
{
    public class Storage : AbstractClient
    {
        internal GrpcClient Client;

        public Storage(GrpcClient client = null)
        {
            this.Client = (client == null) ? new GrpcClient(GetChannel()) : client;
        }
        public Bucket Bucket(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException("bucketName");
            }
            return new Bucket(this, bucketName);
        }
    }
    public class Bucket
    {
        internal Storage Storage;
        public string Name { get; private set; }

        internal Bucket(Storage storage, string name)
        {
            this.Storage = storage;
            this.Name = name;
        }
        public File File(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            return new File(Storage, this, key);
        }
    }
    public class File
    {
        internal Storage Storage;
        internal Bucket Bucket;
        public string Key { get; private set; }

        internal File(Storage storage, Bucket bucket, string key)
        {
            this.Storage = storage;
            this.Bucket = bucket;
            this.Key = key;
        }
        public void Write(byte[] body)
        {
            var request = new StorageWriteRequest
            {
                BucketName = Bucket.Name,
                Key = this.Key,
                Body = ByteString.CopyFrom(body)
            };
            try
            {
                Storage.Client.Write(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
        public byte[] Read()
        {
            var request = new StorageReadRequest
            {
                BucketName = Bucket.Name,
                Key = this.Key
            };
            try
            {
                var response = Storage.Client.Read(request);
                return response.Body.ToByteArray();
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
        public void Delete()
        {
            var request = new StorageDeleteRequest
            {
                BucketName = Bucket.Name,
                Key = this.Key
            };
            try
            {
                Storage.Client.Delete(request);
            }
            catch (Grpc.Core.RpcException re)
            {
                throw NitricException.FromRpcException(re);
            }
        }
        public override string ToString()
        {
            return GetType().Name + "[key=" + Key + "\nbucket=" + Bucket.Name + "]";
        }
    }
}