using Shuile.Core.Framework;
using Shuile.Framework;

using UnityEngine;

namespace Shuile.Gameplay.Event
{
    public class OldEnemyDieEvent : GlobalEvent<OldEnemyDieEvent, GameObject>
    {
    }

    public class OldEnemySpawnEvent : GlobalEvent<OldEnemySpawnEvent, GameObject>
    {
    }

    public class EnemyHurtEvent : ITypeEvent
    {
        public GameObject enemy;
    }
}
