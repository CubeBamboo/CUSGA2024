using UnityEngine;

namespace CbUtils.Event
{
    public class MonoSpawnEventTrigger : MonoBehaviour
    {
        public event System.Action Awake_Evt, OnEnable_Evt, Start_Evt;
        public event System.Action OnDisable_Evt, OnDestroye_Evt;

        private void Awake() => Awake_Evt?.Invoke();
        private void OnEnable() => OnEnable_Evt?.Invoke();
        private void Start() => Start_Evt?.Invoke();
        private void OnDisable() => OnDisable_Evt?.Invoke();
        private void OnDestroy() => OnDestroye_Evt?.Invoke();
    }
}
