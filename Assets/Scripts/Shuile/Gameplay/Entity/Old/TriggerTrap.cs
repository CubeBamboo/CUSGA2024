/*using System.Collections.Generic;

using UnityEngine;

namespace Shuile.Gameplay
{
    public abstract class TriggerTrap : MonoBehaviour, IJudgeable
    {
        protected HashSet<Vector3Int> colliders = new();

        protected virtual void Awake()
        {
            // Register OnEntityMove
        }

        public void Judge(int frame, bool force)
        {
            Recheck();
        }

        // Call from outside
        private void OnEntityMove(GameObject other, Vector3Int from, Vector3Int to)
        {
            var beforeIn = colliders.Contains(from);
            var nowIn = colliders.Contains(to);
            if (!(beforeIn ^ nowIn))
                return;

            if (beforeIn)
                OnExit(other);
            else
                OnEnter(other);
        }

        public abstract void Recheck();

        public abstract void OnEnter(GameObject other);
        public abstract void OnExit(GameObject other);
    }
}
*/