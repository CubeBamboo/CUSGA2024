using CbUtils.Extension;
using CbUtils.Kits.Tasks;
using Shuile.Core.Framework;
using UnityEngine;

namespace Shuile
{
    public static class Extension
    {
        public static GameObject InstantiateGlobalGameObject(this ModuleContainer mod)
        {
            return
                Object.Instantiate(Resources.Load<GameObject>("GlobalGameObject"))
                    .SetDontDestroyOnLoad();
        }

        public static void InitializeBusyScreenByGlobalGameObject(this TaskBus taskBus, GameObject globalGameObject)
        {
            var busyScreen = globalGameObject.GetChildByPath<IBusyScreen>("GlobalCanvas/TaskBusyScreen");
            Debug.Log("Use BusyScreen: " + busyScreen);
            taskBus.Initialize(busyScreen);
        }
    }
}
