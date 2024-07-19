using CbUtils.Extension;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile.Utils
{
    public class MarkedZone2D : MonoBehaviour
    {
        public bool autoDestroyCollider = true;
        private Collider2D[] colliders;

        public Bounds[] MarkedBounds { get; private set; }

        private void Awake()
        {
            colliders = GetComponents<Collider2D>();
            RefreshBoundsList();

            foreach (var coll in colliders)
            {
                if (autoDestroyCollider)
                {
                    coll.Destroy();
                }
            }
        }

        public void RefreshBoundsList()
        {
            MarkedBounds = new Bounds[colliders.Length];
            for (var i = 0; i < colliders.Length; i++)
            {
                MarkedBounds[i] = colliders[i].bounds;
            }
        }

        public Vector2 RandomPosition()
        {
            if (MarkedBounds.Length == 0)
            {
                return Vector2.zero;
            }

            var randomItem = MarkedBounds[URandom.Range(0, MarkedBounds.Length)];
            return new Vector2(URandom.Range(randomItem.min.x, randomItem.max.x),
                URandom.Range(randomItem.min.y, randomItem.max.y));
        }
    }
}
