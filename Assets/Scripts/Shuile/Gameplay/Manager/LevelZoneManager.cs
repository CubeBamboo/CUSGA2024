using CbUtils;
using Shuile.Utils;
using UnityEngine;

namespace Shuile.Gameplay.Manager
{
    public class LevelZoneManager : MonoNotAutoSpawnSingletons<LevelZoneManager>
    {
        [SerializeField] private MarkedZone2D markedZone;

        public Vector2 RandomValidPosition()
        {
            return markedZone.RandomPosition();
        }
    }
}
