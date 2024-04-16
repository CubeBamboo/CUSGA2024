using CbUtils;
using Shuile.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [RequireComponent(typeof(MarkedZone2D))]
    public class LevelZoneManager : MonoSingletons<LevelZoneManager>
    {
        private MarkedZone2D markedZone;

        protected override void OnAwake()
        {
            markedZone = GetComponent<MarkedZone2D>();
        }

        public Vector2 RandomValidPosition()
            => markedZone.RandomPosition();

#if UNITY_EDITOR
        protected override void OnInstanceCall(bool isNewObject)
        {
            if (isNewObject)
            {
                Debug.LogWarning("level zone manager is auto create, you may forget to attach it to the map gameobject");
            }
        }
#endif
    }
}
