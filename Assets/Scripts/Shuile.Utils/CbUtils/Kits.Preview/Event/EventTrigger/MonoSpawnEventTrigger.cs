using UnityEngine;

namespace CbUtils.Event
{
    public class MonoSpawnEventTrigger : MonoBehaviour
    {
        public event System.Action AwakeEvt, OnEnableEvt, StartEvt;
        public event System.Action OnDisableEvt, OnDestroyeEvt;

        private void Awake() => AwakeEvt?.Invoke();
        private void OnEnable() => OnEnableEvt?.Invoke();
        private void Start() => StartEvt?.Invoke();
        private void OnDisable() => OnDisableEvt?.Invoke();
        private void OnDestroy() => OnDestroyeEvt?.Invoke();
    }
}
