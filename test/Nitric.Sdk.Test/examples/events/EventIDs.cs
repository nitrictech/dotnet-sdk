// [START import]

using System.Collections.Generic;
using Nitric.Sdk.Event;

// [END import]

namespace Examples
{
    class EventIdsExample
    {
        public static void EventIdsTopic()
        {
            // [START snippet]
            //Build the payload
            var examplePayload = new Dictionary<string, string>();
            examplePayload.Add("Content", "of event");

            var topic = new EventsClient().Topic("my-topic");

            //Publish the topic
            topic.Publish(new Event { Id = "1234", Payload = examplePayload });
            // [END snippet]
        }
    }
}
