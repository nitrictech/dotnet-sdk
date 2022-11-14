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
using Nitric.Proto.Storage.v1;
using ProtoFile = Nitric.Proto.Storage.v1.File;


namespace Nitric.Sdk.Storage
{
    /// <summary>
    /// A reference to a bucket in the storage service.
    /// </summary>
    public class Bucket
    {
        private readonly Storage storage;

        /// <summary>
        /// The name of the bucket.
        /// </summary>
        public string Name { get; private set; }

        internal Bucket(Storage storage, string name)
        {
            this.storage = storage;
            this.Name = name;
        }

        /// <summary>
        /// Create a reference to a file in the bucket.
        /// </summary>
        /// <param name="key">The files name/path</param>
        /// <returns>The file reference.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public File File(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return new File(storage, this, key);
        }

        /// <summary>
        /// Get a list of files in a bucket.
        /// </summary>
        /// <returns>All the files in the bucket as Nitric file references.</returns>
        public List<File> Files()
        {
            var request = new StorageListFilesRequest
            {
                BucketName = this.Name,
            };

            var resp = this.storage.Client.ListFiles(request);

            List<File> files = new List<File>();

            foreach (ProtoFile file in resp.Files)
            {
                files.Add(new File(this.storage, this, file.Key));
            }

            return files;
        }
    }
}
