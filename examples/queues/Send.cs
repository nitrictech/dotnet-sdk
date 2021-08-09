// [START import]
using Nitric.Api.Queue;
// [END import]

namespace Examples
{
    class SendExample
    {
        public static void SendTask()
        {
            // [START snippet]
            var queue = new Queues().Queue("my-queue");

            var task = Task.NewBuilder()
                .Id("my-task")
                .Build();

            queue.Send(task);
            // [END [snippet]]
        }
    }
}