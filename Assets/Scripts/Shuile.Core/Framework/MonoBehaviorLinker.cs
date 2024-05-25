using CbUtils.Event;
using CbUtils.Extension;
using UnityEngine;

namespace Shuile.Core.Framework
{
    public class MonoBehaviorLinker : MonoBehaviour
    {
        public object LinkedObject;
        public string LinkedObjectTypeName;

        private MonoSpawnEventTrigger _spawnEventTrigger;
        private MonoUpdateEventTrigger _updateEventTrigger;
        public MonoSpawnEventTrigger SpawnEventTrigger => _spawnEventTrigger = _spawnEventTrigger != null ? _spawnEventTrigger : gameObject.GetOrAddComponent<MonoSpawnEventTrigger>();
        public MonoUpdateEventTrigger UpdateEventTrigger => _updateEventTrigger = _updateEventTrigger != null ? _updateEventTrigger : gameObject.GetOrAddComponent<MonoUpdateEventTrigger>();
    }

    public static class MonoBehaviorLinkerExtensions
    {
        public static MonoBehaviorLinker LinkToMonoBehavior(this object self, GameObject target = null, bool isDestroyOnLoadScene = true)
        {
            var name = self.GetType().Name;
            if(!target)
            {
                target = new GameObject($"MonoBehaviorLinker: {name}");
                if(!isDestroyOnLoadScene)
                {
                    target.transform.SetParent(null);
                    Object.DontDestroyOnLoad(target);
                }
            }

            var linker = target.GetOrAddComponent<MonoBehaviorLinker>();
            linker.LinkedObjectTypeName = name;
            linker.LinkedObject = self;
            Debug.Log("MonoBehaviorLinker Create");
            return linker;
        }
        public static GameObject LinkToGameObject(this object self, GameObject target = null, bool isDestroyOnLoadScene = true) => LinkToMonoBehavior(self, target, isDestroyOnLoadScene).gameObject;
    }
}
