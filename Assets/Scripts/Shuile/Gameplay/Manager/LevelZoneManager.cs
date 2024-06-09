using CbUtils;
using Shuile.Utils;
using UnityEngine;

namespace Shuile.Gameplay.Manager
{
    [RequireComponent(typeof(MarkedZone2D))]
    public class LevelZoneManager : MonoNotAutoSpawnSingletons<LevelZoneManager>
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
