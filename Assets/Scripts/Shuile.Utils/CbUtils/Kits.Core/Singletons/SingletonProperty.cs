using UnityEngine;

namespace CbUtils.Unity
{
    public class MonoSingletonProperty<T> where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance => instance = instance ? instance : SingletonCreator.GetMonoBehaviourSingletons<T>();
    }
}
