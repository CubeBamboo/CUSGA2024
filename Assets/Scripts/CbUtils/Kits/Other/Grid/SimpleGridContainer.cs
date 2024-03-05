using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CbUtils
{
    /// <summary>
    /// [early access] it need a gridlayout component, it defines the bounds and the content
    /// </summary>
    /// <typeparam name="T">content type</typeparam>
    [RequireComponent(typeof(GridLayout))]
    public class SimpleGridContainer<T> : MonoBehaviour
    {
        public int width, height;
        private GridLayout mGrid { get; set; }

        /// <summary>
        /// not recommend to access directly, you'll lose the bounds check.
        /// </summary>
        public Dictionary<Vector3Int, T> contents { get; protected set; }

        public Vector3 cellSize => mGrid.cellSize;

        public void Awake()
        {
            mGrid = GetComponent<GridLayout>();
            contents = new();
        }

        public void OnDrawGizmosSelected()
        {
            if (!EditorApplication.isPlaying) return;

            //TODO: in editor...
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cellSize = new Vector3(1, 1, 1); var offset = mGrid.transform.position + cellSize / 2;
                    var pos = offset + cellSize.x * new Vector3(i, j, 0);
                    Gizmos.color = HasContent(new Vector3Int(i, j)) ? Color.red : Color.yellow;
                    Gizmos.DrawWireCube(pos, cellSize);
                }
            }
        }

        public bool IsOutOfBound(Vector3Int pos) => pos.x < 0 || pos.y < 0 || pos.x >= width || pos.y >= height;

        public bool HasContent(Vector3Int pos) => contents.ContainsKey(pos);

        public T GetContent(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return default;
            }

            return contents.GetValueOrDefault(pos, default);
        }

        public bool IsHaveBlock(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            return contents.ContainsKey(pos);
        }

        public Vector3Int WorldToCell(Vector3 pos) => mGrid.WorldToCell(pos);
        public Vector3 CellToWorld(Vector3Int pos) => mGrid.CellToWorld(pos) + mGrid.cellSize / 2;

        public bool AddContent(Vector3Int pos, T go)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            if (!contents.TryAdd(pos, go))
            {
                Debug.LogWarning($"already have contents in {pos}");
                return false;
            }
            else
                return true;

        }

        /// <param name="OnRemove">OnRemove will call before remove. Parameter is the content you want to remove.</param>
        /// <returns>is successful</returns>
        public bool RemoveContent(Vector3Int pos, System.Action<T> OnRemove = null)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            //not exist
            if (!contents.ContainsKey(pos))
            {
                Debug.Log($"Grid RemoveContent(): no gameobject in {pos}");
                return false;
            }

            OnRemove?.Invoke(GetContent(pos));
            contents.Remove(pos);
            return true;
        }

        public bool ModifyContent(Vector3Int pos, T newContent, System.Action<T> OnRemove = null)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            if (!contents.ContainsKey(pos))
            {
                Debug.LogWarning($"Grid ModifyContent(): no gameobject in {pos}");
                return false;
            }
            else
            {
                OnRemove?.Invoke(contents[pos]);
            }

            contents[pos] = newContent;
            return true;
        }

        public void ClearAllContent(System.Action<T> OnRemove = null)
        {
            foreach (var item in contents)
            {
                OnRemove?.Invoke(item.Value);
            }

            contents.Clear();
        }
    }

    public static class SimpleGridExt
    {
        public static Vector3Int RandomPos<T>(this SimpleGridContainer<T> simpleGrid)
        {
            return new Vector3Int()
            {
                x = UnityEngine.Random.Range(0, simpleGrid.width),
                y = UnityEngine.Random.Range(0, simpleGrid.height),
                z = 0
            };
        }

        public static void AddOrModifyContent<T>(this SimpleGridContainer<T> simpleGrid, Vector3Int pos, T content, System.Action<T> OnRemove = null)
        {
            if (simpleGrid.IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return;
            }

            if (!simpleGrid.HasContent(pos))
            {
                simpleGrid.AddContent(pos, content);
            }
            else
            {
                simpleGrid.ModifyContent(pos, content, OnRemove);
            }
        }
    }
}
