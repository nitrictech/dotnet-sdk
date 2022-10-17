// [START import]

using Nitric.Sdk.Queue;

// [END import]

namespace Examples
{
    class SendExample
    {
        public static void SendTask()
        {
            // [START snippet]
            var queue = new QueuesClient().Queue("my-queue");

            var task = new Task { Id = "my-task" };

            queue.Send(task);
            // [END snippet]
        }
    }
}
