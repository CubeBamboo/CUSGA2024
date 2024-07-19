using System;
using UnityEngine;

namespace CbUtils.Event
{
    public class MonoUpdateEventTrigger : MonoBehaviour
    {
        private void Update()
        {
            UpdateEvt?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateEvt?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateEvt?.Invoke();
        }

        public event Action UpdateEvt, FixedUpdateEvt, LateUpdateEvt;
    }
}
