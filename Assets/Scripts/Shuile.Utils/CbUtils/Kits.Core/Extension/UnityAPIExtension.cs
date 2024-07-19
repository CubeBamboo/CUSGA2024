using UnityEngine;

namespace CbUtils.Extension
{
    public static class UnityAPIExtension
    {
        #region Color

        public static Color With(this Color self, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r ?? self.r, g ?? self.g, b ?? self.b, a ?? self.a);
        }

        #endregion

        #region Log

        public static void DumpToUnityLogger(this object obj)
        {
            Debug.Log(obj);
        }

        #endregion

        #region MonoBehaviour

        public static MonoBehaviour SpawnNew(this MonoBehaviour mono)
        {
            var go = new GameObject(mono.name);
            go.AddComponent(mono.GetType());
            return mono;
        }

        #endregion

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

        public static GameObject SetScale(this GameObject go, Vector3 localScale)
        {
            go.transform.localScale = localScale;
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

        public static void DestroySafe(this Object go)
        {
            if (go)
            {
                Object.Destroy(go);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent<T>(out var component))
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public static GameObject GetChildByPath(this GameObject go, string namePath)
        {
            var child = go.transform.Find(namePath);
            return child == null ? null : child.gameObject;
        }

        public static T GetChildByPath<T>(this GameObject go, string namePath)
        {
            var child = go.transform.Find(namePath);
            return child == null ? default : child.GetComponent<T>();
        }

        public static GameObject SetDontDestroyOnLoad(this GameObject go)
        {
            go.transform.SetParent(null);
            Object.DontDestroyOnLoad(go);
            return go;
        }

        #endregion

        #region Transform

        #endregion

        #region Physics

        public static RaycastHit2D DebugLineForRayCast2D(Vector2 origin, Vector2 direction,
            float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers)
        {
            var hit = Physics2D.Raycast(origin, direction, distance, layerMask);
            Debug.DrawLine(origin, origin + (direction * distance), hit ? Color.red : Color.green, 0.1f);
            return hit;
        }

        public static Collider2D GizmosSphereForOverlapCircle2D(Vector2 origin, float radius,
            int layerMask = Physics2D.DefaultRaycastLayers)
        {
            var hit = Physics2D.OverlapCircle(origin, radius, layerMask);
            Gizmos.color = hit ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(origin, radius);
            return hit;
        }

        #endregion
    }

    public static class GameObjectExtension
    {
    }
}
