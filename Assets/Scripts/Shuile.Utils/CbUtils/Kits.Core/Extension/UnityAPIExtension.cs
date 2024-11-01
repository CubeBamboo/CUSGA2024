using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

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

        #region Component

        private static Transform Find(this Transform trans, Func<Transform, bool> pred)
        {
            foreach (Transform tran in trans)
            {
                if (pred(tran))
                {
                    return tran;
                }
            }
            return null;
        }

        private static bool CompareStringIgnoreCaseAndSpace(string l, string r)
        {
            l = l.Replace(" ", string.Empty);
            r = r.Replace(" ", string.Empty);
            return l.Equals(r, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] GetTransformPath(Transform transform)
        {
            var path = new System.Collections.Generic.List<string>();
            while (transform != null)
            {
                path.Add(transform.name);
                transform = transform.parent;
            }
            path.Reverse();
            return path.ToArray();
        }

        private static T ResolveComponent<T>(Transform transform) where T : Component
        {
            if (!transform) throw new NullReferenceException("transform is null");
            if (transform.TryGetComponent<T>(out var component)) return component;
            var components = transform.GetComponents<Component>();

            var transformPath = GetTransformPath(transform);
            var msg = new StringBuilder($"component not match - trying to get {typeof(T)} from {transform.name}\n");
            msg.AppendLine("Hint: transform path");
            msg.AppendJoin(" -> ", transformPath);
            msg.AppendLine();
            msg.AppendLine("Hint: component list");
            foreach (var c in components)
            {
                msg.AppendLine($" - {c.GetType()}");
            }
            throw new InvalidCastException(msg.ToString());
        }

        /// <summary>
        /// path is separated by '/', ignore case and space
        /// </summary>
        public static Transform FindChildInPath(this Transform obj, string path, bool throwIfNull = true)
        {
            if (!obj) return null;
            var toFinds = path.Split('/');

            var ret = obj;
            foreach (var find in toFinds)
            {
                ret = ret.Find(x => x.name.Replace(" ", string.Empty).Equals(find, StringComparison.OrdinalIgnoreCase));
                if (ret) continue;

                if(throwIfNull) throw new Exception($"Cannot find child {find} with full path {path}");
                return null;
            }

            return ret;
        }

        /// <summary>
        /// get child in any depth, ignore case and space, throw if null
        /// </summary>
        private static Transform FindFirstChild(Transform obj, string childName, bool throwIfNull = true)
        {
            if (!obj) return null;
            var find = FindChildWithName(obj, childName);
            if (!find && throwIfNull)
            {
                throw new Exception($"Cannot find child with name {childName}");
            }
            return find;

            Transform FindChildWithName(Transform root, string name)
            {
                if (!root) return null;
                foreach (Transform child in root)
                {
                    if (CompareStringIgnoreCaseAndSpace(child.name, name))
                    {
                        return child;
                    }

                    var found = FindChildWithName(child, name);
                    if (found) return found;
                }
                return null;
            }
        }

        /// <summary>
        /// get child in any depth, ignore case and space, throw if null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetChildByName(this Component component, string childName)
        {
            return FindFirstChild(component.transform, childName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetChildByName<T>(this Component component, string childName) where T : Component
        {
            return ResolveComponent<T>(GetChildByName(component.transform, childName));
        }

        /// <summary>
        /// path is separated by '/', ignore case and space
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetChildByPath(this Component component, string path)
        {
            return FindChildInPath(component.transform, path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetChildByPath<T>(this Component component, string path) where T : Component
        {
            return ResolveComponent<T>(GetChildByPath(component.transform, path));
        }

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

        public static void LogComponentList(this GameObject go, in StringBuilder builder)
        {
            builder.AppendLine($"Components of {go.name}: ");
            var components = go.GetComponents<Component>();
            foreach (var component in components)
            {
                builder.AppendLine(component.ToString());
            }
        }

        public static void LogGameObjectTree(GameObject go, int depth, in StringBuilder builder)
        {
            builder.Append('-', depth * 2);
            builder.AppendLine($"{go.name} - active_{go.activeSelf}");
            foreach (Transform child in go.transform)
            {
                LogGameObjectTree(child.gameObject, depth + 1, builder);
            }
        }

        public static void LogSceneTree(in StringBuilder builder)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                LogGameObjectTree(rootObject, 0, builder);
            }
        }
    }

    public static class GameObjectExtension
    {
    }
}
