using Shuile.Framework;

using UnityEngine;

namespace Shuile.Gameplay.Event
{
    public class EnemyDieEvent : GlobalEvent<EnemyDieEvent, GameObject>
    {
    }

    public class EnemySpawnEvent : GlobalEvent<EnemySpawnEvent, GameObject>
    {
    }

    public class EnemyHurtEvent : GlobalEvent<EnemyHurtEvent, GameObject>
    {
    }
}
