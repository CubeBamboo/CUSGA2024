using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public class UpdateEventMono : MonoBehaviour
    {
        public event System.Action OnUpdate, OnFixedUpdate, OnLateUpdate;

        private void Update() => OnUpdate?.Invoke();
        private void FixedUpdate() => OnFixedUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
    }
}
