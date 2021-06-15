// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
﻿using System;
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
        public static Struct ObjectToStruct(object obj)
        {
            string json = ObjToJson(obj);
            return JsonParser.Default.Parse<Struct>(json);
        }

        public static string GetEnvVar(string variable, string defaultValue = "")
        {
            var envVar = Environment.GetEnvironmentVariable(variable);
            return string.IsNullOrEmpty(envVar) ? defaultValue : envVar;
        }

        public static string ObjToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static object JsonToObj(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static object JsonToObj(byte[] json)
        {
            return JsonConvert.DeserializeObject(json.ToString());
        }

        public static Dictionary<string, object> ObjToDict(object obj)
        {
            return (Dictionary<string, object>) JsonToObj(ObjToJson(obj));
        }

        public static Dictionary<string, List<string>> NameValueCollecToDict(NameValueCollection col)
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k].Split(',').ToList());
            }
            return dict;
        }
    }
}