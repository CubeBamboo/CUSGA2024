using UnityEngine;

namespace CbUtils.Preview.Event
{
    public class UpdateEventMono : MonoBehaviour
    {
        public event System.Action OnStart, OnDestroyed;
        public event System.Action OnUpdate, OnFixedUpdate, OnLateUpdate;

        private void Start()=> OnStart?.Invoke();
        private void OnDestroy()=> OnDestroyed?.Invoke();
        private void Update() => OnUpdate?.Invoke();
        private void FixedUpdate() => OnFixedUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
    }
}
