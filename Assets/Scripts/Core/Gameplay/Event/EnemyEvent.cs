using Shuile.Core.Framework;
using UnityEngine;

namespace Shuile.Gameplay.Event
{
    internal class EnemySpawnEvent : ITypeEvent
    {
        public GameObject enemy;
    }

    internal class EnemyDieEvent : ITypeEvent
    {
        public GameObject enemy;
    }

    internal class EnemyHurtEvent : ITypeEvent
    {
        public GameObject enemy;
    }
}
