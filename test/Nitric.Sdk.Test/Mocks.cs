using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Nitric.Sdk.Test
{
    internal class FakeAsyncStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly List<T> results;
        private int index;

        public FakeAsyncStreamReader(List<T> results)
        {
            index = 0;
            this.results = results;
        }

        public T Current => results[index];

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (index == results.Count - 1)
            {
                return Task.FromResult(false);
            }

            index += 1;

            return Task.FromResult(true);
        }
    }

    internal class FakeClientStreamWriter<T> : IClientStreamWriter<T>
    {
        WriteOptions IAsyncStreamWriter<T>.WriteOptions { get => WriteOptions.Default; set {} }

        public Task CompleteAsync()
        {
            return Task.FromResult(true);
        }

        public Task WriteAsync(T message)
        {
            return Task.FromResult(true);
        }
    }
}