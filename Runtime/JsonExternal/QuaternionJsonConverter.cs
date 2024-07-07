using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mig
{
    public class QuaternionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            float x = (float)jo["x"];
            float y = (float)jo["y"];
            float z = (float)jo["z"];
            float w = (float)jo["w"];
            return new Quaternion(x, y, z, w);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Quaternion quaternion = (Quaternion)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(quaternion.x);
            writer.WritePropertyName("y");
            writer.WriteValue(quaternion.y);
            writer.WritePropertyName("z");
            writer.WriteValue(quaternion.z);
            writer.WritePropertyName("w");
            writer.WriteValue(quaternion.w);
            writer.WriteEndObject();
        }
    }
}