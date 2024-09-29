using UnityEngine;

namespace Plugins.Framework
{
    public class MonoContainer : MonoBehaviour
    {
        public readonly RuntimeContext Context;

        public MonoContainer()
        {
            Context = new RuntimeContext { Reference = this };
        }
    }
}