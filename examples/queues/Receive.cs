// [START import]
using Nitric.Api.Queue;
// [END import]

namespace Examples
{
    class ReceiveExample
    {
        public static void ReceiveTask()
        {
            // [START snippet]
            var queue = new Queues().Queue("my-queue");

            var tasks = queue.Receive(10);

            foreach (ReceivedTask task in tasks)
            {
                // Process tasks

                // Complete the task
                task.Complete();
            }
            // [END snippet]
        }
    }
}