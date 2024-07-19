using System.Runtime.CompilerServices;
using UnityEngine;

namespace Shuile
{
    public static class VectorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 With(this Vector3 vec, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vec.x, y ?? vec.y, z ?? vec.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 With(this Vector2 vec, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vec.x, y ?? vec.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int With(this Vector3Int vec, int? x = null, int? y = null, int? z = null)
        {
            return new Vector3Int(x ?? vec.x, y ?? vec.y, z ?? vec.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color With(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithAlpha(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }
    }
}
