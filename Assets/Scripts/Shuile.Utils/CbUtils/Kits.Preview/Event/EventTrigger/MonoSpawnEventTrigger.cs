using System;
using UnityEngine;

namespace CbUtils.Event
{
    public class MonoSpawnEventTrigger : MonoBehaviour
    {
        private void Awake()
        {
            AwakeEvt?.Invoke();
        }

        private void Start()
        {
            StartEvt?.Invoke();
        }

        private void OnEnable()
        {
            OnEnableEvt?.Invoke();
        }

        private void OnDisable()
        {
            OnDisableEvt?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyeEvt?.Invoke();
        }

        public event Action AwakeEvt, OnEnableEvt, StartEvt;
        public event Action OnDisableEvt, OnDestroyeEvt;
    }
}
