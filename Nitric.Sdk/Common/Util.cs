using System;
using Struct = Google.Protobuf.WellKnownTypes.Struct;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nitric.Api.Common
{
    class Util
    {
        public static Dictionary<string, string> MessageToDict()
        {
        }

        public static Struct ObjectToStruct(Object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonParser.Default.Parse<Struct>(json);
        }

        public static string GetEnvVar(string variable, string defaultValue = "")
        {
            var envVar = Environment.GetEnvironmentVariable(variable);
            return string.IsNullOrEmpty(envVar) ? defaultValue : envVar;
        }
    }
}