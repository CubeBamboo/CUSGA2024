using CbUtils;
using Shuile.Utils;
using UnityEngine;

namespace Shuile.Gameplay.Manager
{
    public interface ILevelZoneManager
    {
        Vector2 RandomValidPosition();
    }

    public class LevelZoneManager : MonoNotAutoSpawnSingletons<LevelZoneManager>, ILevelZoneManager
    {
        [SerializeField] private MarkedZone2D markedZone;

        public Vector2 RandomValidPosition()
        {
            return markedZone.RandomPosition();
        }
    }
}
