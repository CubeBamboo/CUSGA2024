using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shuile
{
    public static class Physics2DUtils
    {
        public static readonly Vector2 zeroAngleVector = new(Mathf.Cos(0), Mathf.Sin(1));

        /// <summary>
        ///     Angle start from Vector2.right
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static Collider2D[] OverlapFan(Vector2 origin, float from, float to, float radius, float arcStep = 0.1f)
        {
            if (from > to)
            {
                throw new ArgumentException($"{nameof(from)} could not bigger than {nameof(to)}");
            }

            while (from < 360f && to - from > 360f)
            {
                from += 360f;
            }

            while (to > 360f && to - from > 360f)
            {
                to -= 360f;
            }

            while (from < 0f)
            {
                from += 360f;
                to += 360f;
            }

            var angleStep = arcStep / (2f * Mathf.PI * radius) * 360f;

            var set = new HashSet<Collider2D>();
            for (var angle = from; angle < to; angle += angleStep)
            {
                Cast(angle);
            }

            Cast(to);

            return set.ToArray();


            void Cast(float angle)
            {
                foreach (var collider in Physics2D
                             .RaycastAll(origin, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), radius)
                             .Select(hit => hit.collider))
                {
                    set.Add(collider);
                }
            }
        }

        public static Collider2D[] OverlapFan(Vector2 origin, Vector2 from, Vector2 to, float radius,
            float arcStep = 0.1f)
        {
            return OverlapFan(origin, Vector2.Angle(zeroAngleVector, from), Vector2.Angle(zeroAngleVector, to), radius,
                arcStep);
        }
    }
}
