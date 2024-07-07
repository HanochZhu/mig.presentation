using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JigSpace;
using System;
using System.Linq;
using Unity.VisualScripting;
using Mig.Core;
using Mig.Snapshot;
using Mig.Model;

namespace Mig
{
    public class ProjectManager : EasySington<ProjectManager>
    {
        private string defaultProjectName = "MigProject";
        private string currentProjectName = null;
        public static string CurrentProjectName
        {
            get =>
#if UNITY_EDITOR
                Instance.currentProjectName?? Instance.defaultProjectName;
#else
                Instance.currentProjectName;
#endif
            set => Instance.currentProjectName = value;
        }

        public static string CurrentProjectFileNameWithExtension => CurrentProjectName + ".mig";

        public static ProjectData CollectionAllSceneDataToProjectData()
        {
            ProjectData projectData = new ProjectData();
            projectData.SnapShotDatas = SnapshotManager.Instance.GetCurrentAllSnapShot();

            var projectModelRoot = ModelManager.Instance.CurrentGameObjectRoot;
            projectData.MigElements = projectModelRoot.GetComponentsInChildrenOnly<MigElementWrapper>()
                .Select(a => a.Elements)
                .SelectMany(list => list)
                .ToList();

            return projectData;
        }


        public void ApplyProjectData(ProjectData data, GameObject currentRoot)
        {
            for (int i = 0; i < data.SnapShotDatas.Count; i++)
            {
                var texPath = Path.Combine(PathManager.GetAccountTempSnapshotTexFolder(), i + ".png");
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(File.ReadAllBytes(texPath));
                data.SnapShotDatas[i].Image = tex;
            }

            SnapshotManager.Instance.SetAllSnapShot(data.SnapShotDatas);

            foreach (var element in data.MigElements)
            {
                if (string.IsNullOrEmpty(element.GameObjectPath))
                {
                    Debug.Log("[Mig] GameObjectPath is null");
                    continue;
                }
                var trans = currentRoot.transform.Find(element.GameObjectPath);
                if (trans == null)
                {
                    Debug.Log($"[Mig] Failed to find {element.GameObjectPath}");
                    continue;
                }
                trans.gameObject.GetOrAddComponent<MigElementWrapper>()
                    .PushBackElement(element);
                element.Wrapper = trans.gameObject.GetOrAddComponent<MigElementWrapper>();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<(string, Texture2D)> GetLocalProjectList()
        {
            var rootFolder = PathManager.GetCachedRootFolder();

            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }

            var directories = Directory.GetDirectories(rootFolder).ToList();

            var accountDir = Directory.GetDirectories(PathManager.GetAccountTempFolder());

            directories.AddRange(accountDir);

            List<(string, Texture2D)> result = new List<(string, Texture2D)>();

            foreach (var directory in directories)
            {
                var directoryName = Path.GetFileName(directory);
                var tex = new Texture2D(50, 50);
                var texturePath = Path.Combine(directory, "Tex", "0.png");
                if (!File.Exists(texturePath)) 
                {
                    result.Add((directoryName, null));
                    continue;
                }
                tex.LoadRawTextureData(File.ReadAllBytes(texturePath));

                result.Add((directoryName, tex));
            }
            return result;
        }

        public async void LoadProjectFileFromWeb(string address, Action<bool> callback)
        {
            // TODO get zip name from the place download file
            var saveZipPath = Path.Combine(PathManager.GetCachedRootFolder(), ProjectManager.CurrentProjectName + ".mig");

            if (!Directory.Exists(PathManager.GetCachedRootFolder()))
            {
                Directory.CreateDirectory(PathManager.GetCachedRootFolder());
            }

            if (File.Exists(saveZipPath))
            {
                File.Delete(saveZipPath);
            }

            var result = await FTPClient.DownloadToFileAsync(address, saveZipPath);

            if (!result)
            {
                Debug.Log($"Failed to download file {address} to {saveZipPath}");
                callback?.Invoke(false);
                return;
            }
            ProjectDeserializer.DeserializerAsync(saveZipPath, callback);
        }
    }

}
