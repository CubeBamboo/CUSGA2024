using System.Runtime.CompilerServices;

using UnityEngine;

namespace Shuile
{
    public static class VectorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 With(this Vector3 vec, float? x = null, float? y = null, float? z = null)
            => new Vector3(x ?? vec.x, y ?? vec.y, z ?? vec.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 With(this Vector2 vec, float? x = null, float? y = null)
            => new Vector2(x ?? vec.x, y ?? vec.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int With(this Vector3Int vec, int? x = null, int? y = null, int? z = null)
            => new(x ?? vec.x, y ?? vec.y, z ?? vec.z);
    }
}
