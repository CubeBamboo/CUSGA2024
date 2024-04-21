using CbUtils;
using Shuile.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [RequireComponent(typeof(MarkedZone2D))]
    public class LevelZoneManager : MonoNonAutoSpawnSingletons<LevelZoneManager>
    {
        private MarkedZone2D markedZone;

        protected override void OnAwake()
        {
            markedZone = GetComponent<MarkedZone2D>();
        }

        public Vector2 RandomValidPosition()
            => markedZone.RandomPosition();
    }
}
