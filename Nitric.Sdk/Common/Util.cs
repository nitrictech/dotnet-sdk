using System;
using Struct = Google.Protobuf.WellKnownTypes.Struct;
using System.Collections.Generic;
using Newtonsoft.Json;
using Google.Protobuf;
using System.Collections.Specialized;
using System.Linq;

namespace Nitric.Api.Common
{
    public class Util
    {
        /*public static Dictionary<string, string> MessageToDict()
        {

        }*/

        public static Struct ObjectToStruct(Object obj)
        {
            string json = ObjToJson(obj);
            return JsonParser.Default.Parse<Struct>(json);
        }

        public static string GetEnvVar(string variable, string defaultValue = "")
        {
            var envVar = Environment.GetEnvironmentVariable(variable);
            return string.IsNullOrEmpty(envVar) ? defaultValue : envVar;
        }

        public static string ObjToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static Object JsonToObj(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
        public static Object JsonToObj(byte[] json)
        {
            return JsonConvert.DeserializeObject(json.ToString());
        }
        public static Dictionary<string, List<string>> NameValueCollecToDict(NameValueCollection col)
        {
            Dictionary<string,List<string>> dict = new Dictionary<string, List<string>>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k].Split(',').ToList());
            }
            return dict;
        }
    }
}