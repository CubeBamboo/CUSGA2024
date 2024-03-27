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
    }
}
