using CbUtils.Extension;
using Shuile.Core.Framework;
using UnityEngine;

namespace Shuile
{
    public static class GlobalCommands
    {
        public static GameObject InstantiateGlobalGameObject()
        {
            return
                Object.Instantiate(Resources.Load<GameObject>("GlobalGameObject"))
                    .SetDontDestroyOnLoad();
        }
    }
}
