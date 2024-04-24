using CbUtils.Extension;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile.Utils
{
    public class MarkedZone2D : MonoBehaviour
    {
        private Collider2D[] colliders;
        private Bounds[] bounds;
        public bool autoDestroyCollider = true;

        private void Awake()
        {
            colliders = GetComponents<Collider2D>();
            RefreshBoundsList();

            foreach (var coll in colliders)
            {
                if(!autoDestroyCollider) coll.Destroy();
            }
        }

        public Bounds[] MarkedBounds => bounds;

        public void RefreshBoundsList()
        {
            bounds = new Bounds[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                bounds[i] = colliders[i].bounds;
            }
        }
        public Vector2 RandomPosition()
        {
            if (bounds.Length == 0) return Vector2.zero;
            var randomItem = bounds[URandom.Range(0, bounds.Length)];
            return new Vector2(URandom.Range(randomItem.min.x, randomItem.max.x),
                               URandom.Range(randomItem.min.y, randomItem.max.y));
        }
    }
}
