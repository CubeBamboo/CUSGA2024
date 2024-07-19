using UnityEngine;

namespace CbUtils.Unity
{
    public class MonoSingletonProperty<T> where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance => instance =
            EnableAutoSpawn && !instance ? SingletonCreator.GetMonoBehaviourSingletons<T>() : instance;

        public static bool EnableAutoSpawn { get; set; } = true;

        public static void InitSingleton(T instance, bool checkSingle = true)
        {
            if (checkSingle && MonoSingletonProperty<T>.instance != null &&
                instance != MonoSingletonProperty<T>.instance)
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            EnableAutoSpawn = false;
            MonoSingletonProperty<T>.instance = instance;
        }
    }
}
