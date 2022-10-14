// [START import]
using System.Collections.Generic;
using Nitric.Sdk.Event;
// [END import]

namespace Examples
{
    class PublishExample
    {
        public static void PublishTopic()
        {
            // [START snippet]
            //Build the payload
            var examplePayload = new Dictionary<string, string>();
            examplePayload.Add("Content", "of event");

            var topic = new Events().Topic("my-topic");

            //Publish the topic
            topic.Publish(
              Event.NewBuilder()
                .Payload(examplePayload)
                .Build()
            );
            // [END snippet]
        }
    }
}
