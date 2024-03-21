using UnityEngine;

namespace Shuile
{
    public static class VectorExtension
    {
        public static Vector3 With(this Vector3 vec, float? x = null, float? y = null, float? z = null)
            => new Vector3(x ?? vec.x, y ?? vec.y, z ?? vec.z);

        public static Vector2 With(this Vector2 vec, float? x = null, float? y = null)
            => new Vector2(x ?? vec.x, y ?? vec.y);
    }
}