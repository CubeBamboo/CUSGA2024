using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CbUtils.Collections
{

    /// <typeparam name="T">content type</typeparam>
    public class HashGridContainer<T> : IPositionGrid, IContainerGrid<T>
    {
        // settings
        public Vector3 cellSize;
        public Vector3 cellGap;
        public Vector3 originPosition;

        public int width, height;

        /// <summary>
        /// not recommend to access directly, you'll lose the bounds check.
        /// </summary>
        public Dictionary<Vector3Int, T> contents = new();

        public event System.Action<Vector3Int, T> OnAdd, OnModify, OnRemove;

        public HashGridContainer(Vector3 originPosition, Vector3 cellSize, Vector3 cellGap, int width = -1, int height = -1)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            this.cellGap = cellGap;
        }

        public HashGridContainer(GridLayout grid, int width = -1, int height = -1)
        {
            this.width = width;
            this.height = height;
            this.originPosition = grid.transform.position;
            this.cellSize = grid.cellSize;
            this.cellGap = grid.cellGap;
        }

        public Vector3 OriginPosition => originPosition;
        public int Width => width;
        public int Height => height;
        public Vector3 CellSize => cellSize;
        public Vector3 CellGap => cellGap;

        public Vector3Int WorldToCell(Vector3 pos) => this.DefaultWorldToCell(pos);
        public Vector3 CellToWorld(Vector3Int pos) => this.DefaultCellToWorld(pos);

        public T Get(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return default;
            }

            return contents.GetValueOrDefault(pos, default);
        }

        public bool Modify(Vector3Int pos, T newContent)
        {
            if (IsOutOfBound(pos))
                throw new IndexOutOfRangeException($"Grid IndexOutOfRange: {pos}");

            if (!contents.ContainsKey(pos))
            {
                Debug.LogWarning($"Grid ModifyContent(): no gameobject in {pos}");
                return false;
            }

            OnModify?.Invoke(pos, newContent);
            contents[pos] = newContent;
            return true;
        }

        public void Clear(System.Action<Vector3Int, T> OnDo = null)
        {
            foreach (var item in contents)
            {
                OnDo?.Invoke(item.Key, item.Value);
            }

            contents.Clear();
        }

        public void ForEach(Action<Vector3Int, T> OnDo)
        {
            foreach (var item in contents)
            {
                OnDo?.Invoke(item.Key, item.Value);
            }
        }

        public bool Add(Vector3Int pos, T content)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            if (contents.ContainsKey(pos))
            {
                Debug.LogWarning($"Grid AddContent(): already has gameobject in {pos}");
                return false;
            }

            OnAdd?.Invoke(pos, content);
            contents.Add(pos, content);
            return true;
        }

        public bool Remove(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            if (!contents.ContainsKey(pos))
            {
                Debug.LogWarning($"Grid RemoveContent(): no gameobject in {pos}");
                return false;
            }

            OnRemove?.Invoke(pos, contents[pos]);
            contents.Remove(pos);
            return true;
        }

        public bool IsValidCell(Vector3Int pos)
            => !IsOutOfBound(pos);

        // custom
        public bool IsOutOfBound(Vector3Int pos) => this.DefaultIsOutOfBound(pos);
        public T this[int x, int y]
        {
            get => Get(new Vector3Int(x, y));
            set => Modify(new Vector3Int(x, y), value);
        }
        public bool HasContent(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            return contents.ContainsKey(pos);
        }
    }

    public static class HashGridContainerExt
    {
        public static void AddOrModifyContent<T>(this HashGridContainer<T> simpleGrid, Vector3Int pos, T content)
        {
            if (simpleGrid.IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return;
            }

            if (!simpleGrid.HasContent(pos))
            {
                simpleGrid.Add(pos, content);
            }
            else
            {
                simpleGrid.Modify(pos, content);
            }
        }


#if UNITY_EDITOR
        public static void OnDrawGizmosSelected<T>(this HashGridContainer<T> self) where T : class
        {
            if (!EditorApplication.isPlaying) return;

            foreach(var item in self.contents)
            {
                var pos = self.CellToWorld(item.Key);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(pos, self.CellSize);
            }
        }
#endif

    }
}
