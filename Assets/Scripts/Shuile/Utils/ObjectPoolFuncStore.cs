using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shuile
{
    public static class ObjectPoolFuncStore
    {
        public static Func<GameObject> GameObjectCreate(GameObject prefab, Transform transform = null)
        {
            return () => Object.Instantiate(prefab, transform);
        }

        public static void GameObjectGet(GameObject obj)
        {
            obj.SetActive(true);
        }

        public static void GameObjectRelease(GameObject obj)
        {
            obj.SetActive(false);
        }

        public static void GameObjectDestroy(GameObject obj)
        {
            Object.Destroy(obj);
        }
    }
}
