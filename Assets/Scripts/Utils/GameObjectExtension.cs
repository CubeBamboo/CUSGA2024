using UnityEngine;

namespace Shuile
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)  // cannot use ??=
                component = gameObject.AddComponent<T>();
            return component;
        }
    }
}