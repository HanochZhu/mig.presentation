using Mig.Core;
using Mig.Snapshot;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mig
{
    public class ProjectData
    {
        public List<SnapShotData> SnapShotDatas = new List<SnapShotData>();

        public List<MigElement> MigElements = new List<MigElement>();

        private static JsonSerializerSettings settings => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new JsonColorConverter() ,new Texture2DJsonConverter(),new Vector3JsonConverter(),new QuaternionJsonConverter()},
            TypeNameHandling = TypeNameHandling.All
        };

        // Todo, refactor is needed
        public static string SerializedCurrentProject()
        {
            var projectData = ProjectManager.CollectionAllSceneDataToProjectData();
            var jsonContent = JsonConvert.SerializeObject(projectData, settings);
            return jsonContent;
        }

        public static ProjectData DeserializeToProjects(string json)
        {
            return JsonConvert.DeserializeObject<ProjectData>(json, settings);
        }
    }
}
