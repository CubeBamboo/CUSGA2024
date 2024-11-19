using CbUtils.Extension;
using CbUtils.Kits.Tasks;
using UnityEngine;

namespace Shuile
{
    public static class Extension
    {
        public static void InitializeBusyScreenByGlobalGameObject(this TaskBus taskBus, GameObject globalGameObject)
        {
            var busyScreen = globalGameObject.GetChildByPath<IBusyScreen>("GlobalCanvas/TaskBusyScreen");
            taskBus.Initialize(busyScreen);
        }
    }
}
