using Shuile.Framework;

using UnityEngine;

namespace Shuile.Gameplay.Event
{
    public class EnemyDieEvent : GlobalEvent<EnemyDieEvent>
    {
    }

    public class EnemySpawnEvent : GlobalEvent<EnemySpawnEvent, GameObject>
    {
    }
}
