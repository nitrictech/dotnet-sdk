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

using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Newtonsoft.Json;
using Struct = Google.Protobuf.WellKnownTypes.Struct;
using Collection = Nitric.Proto.Document.v1.Collection;

namespace Nitric.Sdk.Common.Util
{
    /// <summary>
    /// Common internal utility functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Convert an object to a protobuf struct
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Struct ObjToStruct(object obj)
        {
            if (obj == null) return null;
            var json = ObjToJson(obj);
            return JsonParser.Default.Parse<Struct>(json);
        }

        private static string ObjToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static T JsonToObj<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Deserialize a UTF-8 JSON string to an object.
        /// </summary>
        /// <param name="json">byte array containing UTF-8 JSON data</param>
        /// <returns>Deserialized object</returns>
        public static T JsonToObj<T>(byte[] json)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json));
        }

        /// <summary>
        /// Convert an object to JSON compatible dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ObjToDict(object obj)
        {
            return JsonToObj<Dictionary<string, object>>(ObjToJson(obj));
        }

        /// <summary>
        /// Determines the depth of the provided collection.
        ///
        /// If the collection has no parent collections the depth will be 1.
        ///     The depth is increased by 1 for each parent.
        /// </summary>
        /// <param name="collection">the collection to check for depth</param>
        /// <returns>the determined depth</returns>
        public static int CollectionDepth(Collection collection)
        {
            var parentDepth = (collection.Parent != null) ? CollectionDepth(collection.Parent.Collection) : 0;
            return 1 + parentDepth;
        }
    }
}
