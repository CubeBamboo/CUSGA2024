using System;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils.Event
{
    public class DestroyEventMono : MonoBehaviour
    {
        private readonly Dictionary<string, Action> actions = new();

        private void OnDestroy()
        {
            foreach (var action in actions)
            {
                action.Value();
            }

            OnDestroyed?.Invoke();
        }

        public event Action OnDestroyed;

        public bool Contains(string label)
        {
            return actions.ContainsKey(label);
        }

        public bool TryAdd(string label, Action action)
        {
            return actions.TryAdd(label, action);
        }

        public bool Remove(string label)
        {
            return actions.Remove(label);
        }
    }

    public static class DestroyEventExtensions
    {
        /// <param name="label">
        ///     use label to identity single action, it will only work in this gameObject. pass null will be set
        ///     to a random label
        /// </param>
        public static bool SetOnDestroy(this GameObject gameObject, Action action, string label = null)
        {
            label ??= action.Method.Name; // TODO: random name with high performance
            if (!gameObject.TryGetComponent<DestroyEventMono>(out var trigger))
            {
                trigger = gameObject.AddComponent<DestroyEventMono>();
            }

            return trigger.TryAdd(label, action);
        }
    }
}
