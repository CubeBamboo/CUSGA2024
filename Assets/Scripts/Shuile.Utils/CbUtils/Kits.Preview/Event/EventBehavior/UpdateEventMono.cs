using System;
using UnityEngine;

namespace CbUtils.Preview.Event
{
    public class UpdateEventMono : MonoBehaviour
    {
        private void Start()
        {
            OnStart?.Invoke();
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }

        public event Action OnStart, OnDestroyed;
        public event Action OnUpdate, OnFixedUpdate, OnLateUpdate;
    }
}
