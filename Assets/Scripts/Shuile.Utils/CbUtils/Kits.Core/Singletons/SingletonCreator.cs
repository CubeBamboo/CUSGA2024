using UnityEngine;

namespace CbUtils.Unity
{
    public static class SingletonCreator
    {
        public static T GetMonoBehaviourSingletons<T>() where T : MonoBehaviour
        {
            var find = Object.FindObjectOfType<T>();
            if(find != null)
            {
                return find;
            }
            else
            {
                var create = new GameObject($"MonoSingletons.{typeof(T).Name}").AddComponent<T>();
                Debug.Log($"Create new singletons: {create.gameObject.name}");
                return create;
            }
        }
    }
}
