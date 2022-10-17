using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nitric.Sdk.Function;
using Nitric.Sdk.Resource;

namespace Nitric.Sdk
{
    internal sealed class Nitric
    {
        private static List<Faas> workers = new List<Faas>();

        private static Dictionary<string, Dictionary<string, BaseResource>> cache =
            new Dictionary<string, Dictionary<string, BaseResource>>();

        internal static void RegisterWorker(Faas worker)
        {
            Nitric.workers.Add(worker);
        }

        public static void Run()
        {
            Task.WaitAll(workers.Select(worker => worker.Start()).ToArray());
        }

        // public static Api Api()
    }
}
