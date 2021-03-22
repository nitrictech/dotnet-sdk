using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nitric.Sdk.v1.Util
{
    class Util
    {
        public static Dictionary<string, string> MessageToDict()
        {
        }
        public static Struct DictToMessage(Dictionary<string, string> dict)
        {
            string json = JsonConvert.SerializeObject(dict);
            return JsonParser.Default.Parse<Struct>(json);
        }
    }
}