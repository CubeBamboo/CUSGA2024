using System.Collections.Generic;
using UnityEngine;

namespace CbUtils.Event
{
    public class DestroyEventMono : MonoBehaviour
    {
        private Dictionary<string, System.Action> actions = new();
        public bool Contains(string label) => actions.ContainsKey(label);
        public bool TryAdd(string label, System.Action action)
            => actions.TryAdd(label, action);
        
        public bool Remove(string label)
            => actions.Remove(label);
        
        private void OnDestroy()
        {
            foreach (var action in actions)
            {
                action.Value();
            }
        }
    }

    public static class DestroyEventExtensions
    {
        /// <param name="label"> use label to identity single action, it will only work in this gameObject. pass null will be set to a random label </param>
        public static bool SetOnDestroy(this GameObject gameObject, System.Action action, string label = null)
        {
            label ??= action.Method.Name; // TODO: random name with high performance
            if (!gameObject.TryGetComponent<DestroyEventMono>(out var trigger))
                trigger = gameObject.AddComponent<DestroyEventMono>();
            return trigger.TryAdd(label, action);
        }
    }
}
