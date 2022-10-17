using System;

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
    }
}
