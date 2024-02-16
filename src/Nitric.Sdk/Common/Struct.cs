using System;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json;
using KindOneofCase = Google.Protobuf.WellKnownTypes.Value.KindOneofCase;
using ProtoStruct = Google.Protobuf.WellKnownTypes.Struct;
using ProtoValue = Google.Protobuf.WellKnownTypes.Value;

namespace Nitric.Sdk.Common
{
    public class Struct
    {
        public static T ToJsonSerializable<T>(ProtoStruct @struct)
        {
            T jsonSerializable = default;
            if (@struct != null)
            {
                JsonFormatter formatter = new JsonFormatter(JsonFormatter.Settings.Default);
                jsonSerializable = JsonConvert.DeserializeObject<T>(formatter.Format(@struct));
            }

            return jsonSerializable;
        }

        public static ProtoStruct FromJsonSerializable<T>(T jsonSerializable)
        {
            ProtoStruct structPayload = null;
            if (jsonSerializable != null)
            {
                var jsonPayload = JsonConvert.SerializeObject(jsonSerializable);
                structPayload = JsonParser.Default.Parse<ProtoStruct>(jsonPayload);
            }

            return structPayload;
        }
    }
}

