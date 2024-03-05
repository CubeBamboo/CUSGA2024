using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CbUtils
{
    //SO as event
    [CreateAssetMenu(fileName = "new Event", menuName = "CustomEventData/VoidEventSO")]
    public class VoidEventSO : ScriptableObject
    {
        public UnityEvent evt;

        public void Raise()
        {
            evt?.Invoke();
        }
    }
}