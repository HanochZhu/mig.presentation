using Mig.Core;
using System.IO;
using UnityEngine;

namespace Mig
{
    public class PathManager
    {

        public static string GetTempRootFolder()                    => Application.persistentDataPath;
        public static string GetCachedRootFolder()                  => Path.Combine(GetTempRootFolder(), "Cache");
        public static string GetAccountTempFolder()                 => Path.Combine(Application.persistentDataPath, AccountManager.GetCurrentAccountID());

        public static string GetAccountCurrentProjectFolder()       => Path.Combine(GetAccountTempFolder(), ProjectManager.CurrentProjectName);
        public static string GetAccountTempModelFolder()            => Path.Combine(GetAccountCurrentProjectFolder(), "Model");
        public static string GetAccountTempSnapshotTexFolder()      => Path.Combine(GetAccountCurrentProjectFolder(), "Tex");
        public static string GetAccountTempSnapshotElementFolder()  => Path.Combine(GetAccountCurrentProjectFolder(), "Element");

        public static string GetDefaultAccountTempSnapshotElementFolder() => Path.Combine(PathManager.GetAccountTempSnapshotElementFolder(), "element.json");

        public static string GetCurrentFTPDirRoot() => FTPClient.GetCurrentFTPDirRoot();

        public static string GetDefaultZipFilePath() => Path.Combine(PathManager.GetTempRootFolder(), AccountManager.GetCurrentAccountID() + ".mig");
    }
}

