using UnityEngine;

namespace CbUtils
{
    public static class UnityAPIExt
    {
        #region UnityEngine.GameObject

        public static T Instantiate<T>(this T go) where T : Object
        {
            return Object.Instantiate(go);
        }

        public static GameObject SetPosition(this GameObject go, Vector3 pos)
        {
            go.transform.position = pos;
            return go;
        }

        public static GameObject SetRotation(this GameObject go, Quaternion rot)
        {
            go.transform.rotation = rot;
            return go;
        }

        public static GameObject SetParent(this GameObject go, Transform parent, bool worldPositionStays = true)
        {
            go.transform.SetParent(parent, worldPositionStays);
            return go;
        }

        public static GameObject SetName(this GameObject go, string name)
        {
            go.name = name;
            return go;
        }

        public static void Destroy(this Object go)
        {
            Object.Destroy(go);
        }

        #endregion

        #region UnityEngine.Events

        public static ICustomUnRegister AddListenerWithCustomUnRegister(this UnityEngine.Events.UnityEvent self, UnityEngine.Events.UnityAction action)
        {
            self.AddListener(action);
            return new CustomUnRegister(() => self.RemoveListener(action));
        }

        #endregion

        #region CbUtils

        public static Vector3Int ToCell(this Vector3 self, IPositionGrid grid)
            => grid.WorldToCell(self);

        public static Vector3 ToWorld(this Vector3Int self, IPositionGrid grid)
            => grid.CellToWorld(self);

        #endregion

        #region Color

        public static Color With(this Color self, float? r = null, float? g = null, float? b = null, float? a = null)
            => new Color(r ?? self.r, g ?? self.g, b ?? self.b, a ?? self.a);

        #endregion

        #region Physics

        /// <summary> warpper for Physics2D.Raycast() </summary>
        public static RaycastHit2D RayCast2DWithDebugLine(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity)
        {
            var hit = Physics2D.Raycast(origin, direction, distance, layerMask, minDepth, maxDepth);
            Debug.DrawLine(origin, origin + direction * distance, hit ? Color.red : Color.green, 0.1f);
            return hit;
        }

        #endregion

    }
}
