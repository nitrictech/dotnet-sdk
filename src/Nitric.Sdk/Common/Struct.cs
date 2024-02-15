using System;
using System.Collections.Generic;
using KindOneofCase = Google.Protobuf.WellKnownTypes.Value.KindOneofCase;
using ProtoStruct = Google.Protobuf.WellKnownTypes.Struct;
using ProtoValue = Google.Protobuf.WellKnownTypes.Value;

namespace Nitric.Sdk.Common
{
    public class Struct
    {
        public static Dictionary<string, object> ToDictionary(ProtoStruct @struct)
        {
            var payload = new Dictionary<string, object>();

            foreach (var kv in @struct.Fields)
            {
                payload.Add(kv.Key, GetValue(kv.Value));
            }

            return payload;
        }

        public static ProtoStruct FromDictionary(Dictionary<string, object> dictionary)
        {
            ProtoStruct payload = new ProtoStruct();

            foreach (KeyValuePair<string, object> kv in dictionary)
            {
                payload.Fields.Add(kv.Key, Struct.ToValue(kv.Value));
            }

            return payload;
        }

        private static ProtoValue ToValue(object val)
        {
            if (val is string str)
            {
                return ProtoValue.ForString(str);
            }
            else if (val is double num)
            {
                return ProtoValue.ForNumber(num);
            }
            else if (val is null)
            {
                return ProtoValue.ForNull();
            }
            else if (val is Dictionary<string, object> dictionary)
            {
                return ProtoValue.ForStruct(Struct.FromDictionary(dictionary));
            }
            else if (val is List<object> list)
            {
                return ProtoValue.ForList(list.ConvertAll(ToValue).ToArray());
            }

            return null;
        }

        private static object GetValue(ProtoValue val)
        {
            switch (val.KindCase)
            {
                case KindOneofCase.StringValue:
                    return val.StringValue;
                case KindOneofCase.NumberValue:
                    return val.NumberValue;
                case KindOneofCase.BoolValue:
                    return val.BoolValue;
                case KindOneofCase.NullValue:
                    return val.NullValue;
                case KindOneofCase.ListValue:
                    var list = new List<object>();

                    foreach (var listVal in val.ListValue.Values)
                    {
                        list.Add(Struct.GetValue(listVal));
                    }

                    return list;
                case KindOneofCase.StructValue:
                    return Struct.ToDictionary(val.StructValue);
                default:
                    return null;
            }
        }
    }
}

