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

        public Texture2D DeCompress(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
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
                writer.WriteNull();  // 指示纹理不可读
                writer.WriteEndObject();
                return;
            }

            byte[] imageData = DeCompress(texture).EncodeToPNG();
            if (imageData == null)
            {
                Debug.LogWarning($"Failed to encode texture '{texture.name}' to PNG.");
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue("Texture2D");
                writer.WritePropertyName("textureData");
                writer.WriteNull();  // 指示纹理编码失败
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
            var jsonObject = JObject.Load(reader);
            var base64String = jsonObject["textureData"]?.ToString();
            if (string.IsNullOrEmpty(base64String))
            {
                Debug.LogError("纹理数据无效");
                return null;  // 返回null表示纹理数据无效
            }

            byte[] imageData = Convert.FromBase64String(base64String);

            Texture2D texture = new Texture2D(2, 2);
            bool isLoaded = texture.LoadImage(imageData);
            if (!isLoaded)
            {
                Debug.LogError("Failed to load image data into Texture2D.");
                return null;
            }

            // 生成唯一的文件名
            string uniqueFileName = GenerateUniqueFileName("savedTexture", "png");
            // 保存纹理到本地
            SaveTextureToFile(texture, uniqueFileName); ;

            Debug.Log("Loaded Texture2D successfully");
            return texture;
        }

        private void SaveTextureToFile(Texture2D texture, string fileName)
        {
            byte[] bytes = texture.EncodeToPNG();
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(path, bytes);
            Debug.Log($"Texture saved to {path}");
        }

        private string GenerateUniqueFileName(string baseName, string extension)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return $"{baseName}_{timestamp}.{extension}";
        }

    }
}