using UnityEngine;

namespace CbUtils.Event
{
    public class MonoUpdateEventTrigger : MonoBehaviour
    {
        public event System.Action UpdateEvt, FixedUpdateEvt, LateUpdateEvt;
        private void Update() => UpdateEvt?.Invoke();
        private void FixedUpdate() => FixedUpdateEvt?.Invoke();
        private void LateUpdate() => LateUpdateEvt?.Invoke();
    }
}
