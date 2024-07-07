using JigSpace;
using Mig.Core;
using Mig.Model;
using Mig.Snapshot;
using System;
using System.Linq;
using UnityEngine;

namespace Mig
{
    public static class MigElementUtils
    {
        public static T GetOrAddCurrentStepElement<T>(this GameObject host) where T : MigElement, new()
        {
            var wrapper = host.GetOrAddComponent<MigElementWrapper>();
            var elements = wrapper.Elements;
            var currentGuid = SnapshotManager.Instance.CurrentSnapshotGuid;

            CheckForFirstElement<T>(wrapper);

            var currentElement = elements.Where(e => e.StepGUID == currentGuid).ToList().Where(a => a is T);// SnapshotManager.Instance.CurrentSnapshotIndex

            if (currentElement.Count() > 1)
            {
                Debug.LogError($"[Mig] Multi-defined {typeof(T)} in same GameObject {host.name}");
                return null;
            }
            var firstElement = currentElement.FirstOrDefault();

            if (firstElement == null)
            {
                firstElement = CreateElementAtWrapper<T>(wrapper, currentGuid);
            }
            return firstElement as T;
        }

        private static void CheckForFirstElement<T>(MigElementWrapper wrapper) where T : MigElement, new()
        {
            var elements = wrapper.Elements;

            if (elements.Count((a) => a is T) != 0)
            {
                return;
            }
            var defaultElement = CreateElementAtWrapper<T>(wrapper, Guid.Empty);

            defaultElement.Record();
        }

        private static T CreateElementAtWrapper<T>(MigElementWrapper wrapper, Guid currentGuid) where T : MigElement, new()
        {
            var element = new T();
            wrapper.PushBackElement(element);
            element.Wrapper = wrapper;
            var gameobjectPath = GameObjectExtensions.GetGameObjectTreePath(wrapper.gameObject, ModelManager.Instance.CurrentGameObjectRoot.transform);
            element.Init(gameobjectPath, currentGuid);
            return element;
        }
    }

}

