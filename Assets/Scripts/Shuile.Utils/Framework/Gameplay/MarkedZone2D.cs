using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile.Utils
{
    public class MarkedZone2D : MonoBehaviour
    {
        private Collider2D[] colliders;
        public bool autoSetToTrigger = true;

        private void Awake()
        {
            colliders = GetComponents<Collider2D>();
            foreach (var coll in colliders)
            {
                coll.isTrigger = autoSetToTrigger;
            }
        }

        public Collider2D[] ColliderForMark => colliders;

        public Vector2 RandomPosition()
        {
            if (colliders.Length == 0) return Vector2.zero;
            var randomColl = colliders[URandom.Range(0, colliders.Length)];
            return new Vector2(URandom.Range(randomColl.bounds.min.x, randomColl.bounds.max.x),
                               URandom.Range(randomColl.bounds.min.y, randomColl.bounds.max.y));
        }
    }
}
