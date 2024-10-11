using Shuile.Core.Framework;
using Shuile.Gameplay.Entity;
using UnityEngine;

namespace Shuile.Gameplay.Event
{
    // maybe not a good idea...
    internal class EnemySpawnEvent : ITypeEvent
    {
        public Enemy enemy;
    }

    internal class EnemyDieEvent : ITypeEvent
    {
        public Enemy enemy;
    }

    internal class EnemyHurtEvent : ITypeEvent
    {
        public Enemy enemy;
    }
}
