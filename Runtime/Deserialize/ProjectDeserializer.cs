using System;
using System.IO;
using System.Threading.Tasks;
using Unity.SharpZipLib.Utils;

namespace Mig
{
    public class ProjectDeserializer
    {
        /// <summary>
        /// TODO, handle exception
        /// </summary>
        /// <param name="srcByteArray"></param>
        /// <param name="callback"></param>
        public static async void DeserializerAsync(string saveZipPath, Action<bool> callback)
        {
            var extractPath = PathManager.GetAccountTempFolder();

            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            var task = Task.Run(() =>
            {
                ZipUtility.UncompressFromZip(saveZipPath, string.Empty, extractPath);
            });

            await task;

            callback?.Invoke(true);
        }
    }
}
