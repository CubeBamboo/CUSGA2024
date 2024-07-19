using System;
using UnityEditor;
using UnityEngine;

namespace CbUtils.Collections
{
    /// <typeparam name="T">content type</typeparam>
    public class ArrayGridContainer<T> : IPositionGrid, IContainerGrid<T> where T : class // nullable?
    {
        // settings

        /// <summary> not recommend to access directly, you'll lose the bounds check. </summary>
        public T[,] contents;

        public ArrayGridContainer(Vector3 originPosition, Vector3 cellSize, Vector3 cellGap, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
            this.OriginPosition = originPosition;
            this.CellGap = cellGap;

            contents = new T[width, height];
        }

        public ArrayGridContainer(GridLayout grid, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            OriginPosition = grid.transform.position;
            CellSize = grid.cellSize;
            CellGap = grid.cellGap;

            contents = new T[width, height];
        }

        public T this[int x, int y]
        {
            get => Get(new Vector3Int(x, y));
            set => Modify(new Vector3Int(x, y), value);
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public T Get(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                return null;
            }

            return contents[pos.x, pos.y];
        }

        public bool Modify(Vector3Int pos, T newContent)
        {
            if (HasContent(pos))
            {
                OnRemove?.Invoke(pos, contents[pos.x, pos.y]);
            }

            contents[pos.x, pos.y] = newContent;
            OnAdd?.Invoke(pos, newContent);
            return true;
        }

        public void ForEach(Action<Vector3Int, T> OnDo)
        {
            for (var i = 0; i < contents.GetLength(0); i++)
            for (var j = 0; j < contents.GetLength(1); j++)
            {
                OnDo?.Invoke(new Vector3Int(i, j), contents[i, j]);
            }
        }

        public void Clear(Action<Vector3Int, T> OnDo = null)
        {
            for (var i = 0; i < contents.GetLength(0); i++)
            {
                for (var j = 0; j < contents.GetLength(1); j++)
                {
                    OnDo?.Invoke(new Vector3Int(i, j), contents[i, j]);
                    contents[i, j] = null;
                }
            }
        }

        public bool Add(Vector3Int pos, T content) // maybe named "register"
        {
            if (IsOutOfBound(pos) || HasContent(pos))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{pos} add to grid container failed.");
#endif
                return false;
            }

            contents[pos.x, pos.y] = content;
            OnAdd?.Invoke(pos, content);
            return true;
        }

        public bool Remove(Vector3Int pos) // maybe named "unregister"
        {
            if (IsOutOfBound(pos) || !HasContent(pos))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{pos} remove from grid container failed.");
#endif
                return false;
            }

            OnRemove?.Invoke(pos, contents[pos.x, pos.y]);
            contents[pos.x, pos.y] = null;
            return true;
        }

        public bool IsValidCell(Vector3Int pos)
        {
            return !IsOutOfBound(pos);
            // maybe add more?
        }

        public Vector3 OriginPosition { get; private set; }

        public Vector3 CellSize { get; private set; }

        public Vector3 CellGap { get; private set; }

        public Vector3Int WorldToCell(Vector3 pos)
        {
            return this.DefaultWorldToCell(pos);
        }

        public Vector3 CellToWorld(Vector3Int pos)
        {
            return this.DefaultCellToWorld(pos);
        }

        public event Action<Vector3Int, T> OnAdd, OnRemove;
        public event Action<Vector3Int, Vector3Int> OnMove;

        // custom
        public bool IsOutOfBound(Vector3Int pos)
        {
            return this.DefaultIsOutOfBound(pos);
        }

        public bool HasContent(Vector3Int pos)
        {
            if (IsOutOfBound(pos))
            {
                return false;
            }

            return contents[pos.x, pos.y] != null;
        }

        public void UpdateConfig(Vector3 originPosition, Vector3 cellSize, Vector3 cellGap, int width, int height)
        {
            this.OriginPosition = originPosition;
            this.CellSize = cellSize;
            this.CellGap = cellGap;

            var isDirty = this.Width != width || this.Height != height;
            this.Width = width;
            this.Height = height;
            if (isDirty)
            {
                contents = new T[width, height];
#if UNITY_EDITOR
                Debug.Log("content array reassigned.");
#endif
            }
        }

        /// <param name="OnMove">para - content to move</param>
        /// <returns>return true if move successfully</returns>
        public bool Move(Vector3Int pos, Vector3Int newPos)
        {
            if (IsOutOfBound(pos))
            {
                Debug.LogWarning($"Grid IndexOutOfRange: {pos}");
                return false;
            }

            if (!HasContent(pos))
            {
                Debug.LogWarning($"Grid MoveContent(): no gameobject in {pos} and it may lead to some bugs.");
            }

            if (HasContent(newPos))
            {
                Debug.LogWarning(
                    $"Grid MoveContent(): already have a gameobject in {newPos} and it may lead to some bugs.");
            }

            contents[newPos.x, newPos.y] = contents[pos.x, pos.y];
            OnMove?.Invoke(pos, newPos);
            contents[pos.x, pos.y] = null;

            return true;
        }

        public bool TryGet(Vector3Int pos, out T res)
        {
            res = null;
            if (IsOutOfBound(pos))
            {
                return false;
            }

            res = contents[pos.x, pos.y];
            return res != null;
        }
    }

    public static class ArrayGridContainerExt
    {
        public static T[,] GetContentContainer<T>(this ArrayGridContainer<T> gridContainer) where T : class
        {
            return gridContainer.contents;
        }

        public static bool TryGet<T>(this ArrayGridContainer<T> gridContainer, Vector3Int pos, out T content)
            where T : class
        {
            content = gridContainer.Get(pos);
            return content != null;
        }

        /// <returns> 1: add, -1: modify </returns>
        public static int AddOrModify<T>(this ArrayGridContainer<T> gridContainer, Vector3Int pos, T content)
            where T : class
        {
            if (gridContainer.HasContent(pos))
            {
                gridContainer.Modify(pos, content);
                return -1;
            }

            gridContainer.Add(pos, content);
            return 1;
        }

        /// <summary>
        ///     Get maximum row which have content in giving column by [0, startRow]. (return -1 if can't find)
        /// </summary>
        public static int GetMaximumRowHaveContent<T>(this ArrayGridContainer<T> gridContainer, int column)
            where T : class
        {
            return GetMaximumRowHaveContent(gridContainer, column, gridContainer.Height - 1);
        }

        public static int GetMaximumRowHaveContent<T>(this ArrayGridContainer<T> gridContainer, int column,
            int startRow) where T : class
        {
            startRow = Mathf.Clamp(startRow, 0, gridContainer.Height - 1);

            for (var i = startRow; i >= 0; i--)
            {
                if (gridContainer.Get(new Vector3Int(column, i)) != null)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Get minimal row which have content in giving column by [0, maxRow). (return maxRow if can't find)
        /// </summary>
        public static int GetMinimalRowHaveContent<T>(this ArrayGridContainer<T> gridContainer, int column, int maxRow)
            where T : class
        {
            maxRow = Mathf.Clamp(maxRow, 0, gridContainer.Height);

            for (var i = 0; i <= maxRow; i++)
            {
                if (gridContainer.Get(new Vector3Int(column, i)) == null)
                {
                    return i - 1;
                }
            }

            return maxRow;
        }

        public static Vector3 ToWorldSize<T>(this ArrayGridContainer<T> gridContainer, int gridCount) where T : class
        {
            return (gridContainer.CellSize * gridCount) + (gridContainer.CellGap * (gridCount - 1));
        }

#if UNITY_EDITOR
        public static void OnDrawGizmosSelected<T>(this ArrayGridContainer<T> self) where T : class
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            for (var i = 0; i < self.Width; i++)
            {
                for (var j = 0; j < self.Height; j++)
                {
                    var offset = self.OriginPosition + (self.CellSize / 2);
                    var step = self.CellSize + self.CellGap;
                    var pos = offset + new Vector3(i * step.x, j * step.y, 0);
                    Gizmos.color =
                        self.HasContent(new Vector3Int(i, j)) ? Color.red : Color.yellow;
                    Gizmos.DrawWireCube(pos, self.CellSize);
                }
            }
        }
#endif
    }
}
