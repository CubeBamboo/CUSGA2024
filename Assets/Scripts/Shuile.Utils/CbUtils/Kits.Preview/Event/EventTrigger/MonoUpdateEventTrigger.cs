using UnityEngine;

namespace CbUtils.Event
{
    public class MonoUpdateEventTrigger : MonoBehaviour
    {
        public event System.Action OnUpdate, OnFixedUpdate, OnLateUpdate;
        private void Update() => OnUpdate?.Invoke();
        private void FixedUpdate() => OnFixedUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
    }
}
