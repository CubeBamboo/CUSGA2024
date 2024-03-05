using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils
{
    //link GameEventData(SO) and UnityEvent as a MonoBehavior
    public class GameEventListener : MonoBehaviour
    {
        public Data.GameEventData eventData;
        public UnityEngine.Events.UnityEvent onEventTriggered;

        private void OnEnable()
        {
            eventData.AddListener(this);
        }

        private void OnDisable()
        {
            eventData.RemoveListener(this);
        }

        public void OnEventTriggered()
        {
            onEventTriggered?.Invoke();
        }
    }
}