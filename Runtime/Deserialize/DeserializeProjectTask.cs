using Mig.Core.TaskPattern;
using Mig.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Mig
{
    public class DeserializeProjectTask : TaskHandlerBase
    {
        public DeserializeProjectTask(Action<bool> taskCallback) : base(taskCallback)
        {

        }

        public override void Execute()
        {
            Debug.Log("[Mig] loading glb complete");

            var root = ModelManager.Instance.CurrentGameObjectRoot;

            var projectJson = File.ReadAllText(PathManager.GetDefaultAccountTempSnapshotElementFolder());

            var data = ProjectData.DeserializeToProjects(projectJson);

            ProjectManager.Instance.ApplyProjectData(data, root);

            this.m_taskCallback?.Invoke(true);

            base.Execute();
        }
    }
}

