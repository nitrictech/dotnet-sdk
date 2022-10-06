// [START import]
using System.Collections.Generic;
using Nitric.Api.Event;
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

            var topic = new Events().Topic("my-topic");

            //Publish the topic
            topic.Publish(
              Event.NewBuilder()
                .Id("1234")
                .Payload(examplePayload)
                .Build()
            );
            // [END snippet]
        }
    }
}