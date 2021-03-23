using System;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nitric.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>()
            {
                { "0", "Cat" },
                { "1", "Dog" }
            };
            TestPayloadStruct(payload);
        }
        static void TestPayloadStruct(Dictionary<string, string> payload)
        {
            string json = JsonConvert.SerializeObject(payload, Formatting.Indented);
            Struct payloadStruct = JsonParser.Default.Parse<Struct>(json);
            Console.WriteLine("The fields:" + payloadStruct.Fields);
        }
    }
}