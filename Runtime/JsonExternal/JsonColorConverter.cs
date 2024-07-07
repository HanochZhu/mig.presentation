using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
namespace Mig
{
    public class JsonColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return Color.clear;

            var jsonObject = JObject.Load(reader);
            var r = (float)jsonObject["r"];
            var g = (float)jsonObject["g"];
            var b = (float)jsonObject["b"];
            var a = (float)jsonObject["a"]; // 默认alpha为1（不透明）  

            return new Color(r, g, b, a);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(color.r);
            writer.WritePropertyName("g");
            writer.WriteValue(color.g);
            writer.WritePropertyName("b");
            writer.WriteValue(color.b);
            writer.WritePropertyName("a");
            writer.WriteValue(color.a);
            writer.WriteEndObject();
        }
    }
}
