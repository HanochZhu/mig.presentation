using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Mig
{
    public class Texture2DJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Texture2D).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Texture2D texture = value as Texture2D;
            if (texture == null)
            {
                throw new ArgumentException("Expected Texture2D object value.");
            }

            if (!texture.isReadable)
            {
                Debug.LogWarning($"Texture '{texture.name}' is not readable. Make sure the texture is set to readable in the texture import settings.");
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue("Texture2D");
                writer.WritePropertyName("textureData");
                writer.WriteNull();  
                writer.WriteEndObject();
                return;
            }

            byte[] imageData = texture.EncodeToPNG();
            if (imageData == null)
            {
                Debug.LogWarning($"Failed to encode texture '{texture.name}' to PNG.");
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue("Texture2D");
                writer.WritePropertyName("textureData");
                writer.WriteNull();  // ÷∏ æŒ∆¿Ì±‡¬Î ß∞‹
                writer.WriteEndObject();
                return;
            }

            string base64String = Convert.ToBase64String(imageData);

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Texture2D");
            writer.WritePropertyName("textureData");
            writer.WriteValue(base64String);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var base64String = reader.ReadAsString();
            if (string.IsNullOrEmpty(base64String))
            {
                Debug.LogError("Fail to get texture json content");
                return null; 
            }

            byte[] imageData = Convert.FromBase64String(base64String);

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadRawTextureData(imageData);

            Debug.Log("Loaded Texture2D successfully");
            return texture;
        }
    }
}