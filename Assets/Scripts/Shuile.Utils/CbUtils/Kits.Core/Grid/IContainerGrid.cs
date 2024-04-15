using UnityEditor;
using UnityEngine;

namespace CbUtils.Collections
{
    /// <summary>
    /// store the content position in grid.
    /// </summary>
    /// <typeparam name="T"> content type </typeparam>
    public interface IContainerGrid<T>
    {
        int Width { get; }
        int Height { get; }

        bool IsValidCell(Vector3Int pos);
        
        T Get(Vector3Int pos);
        /// <returns> false - already have content in pos </returns>
        bool Add(Vector3Int pos, T content);
        /// <returns> false - have nothing in pos </returns>
        bool Remove(Vector3Int pos);
        bool Modify(Vector3Int pos, T newContent);
        void ForEach(System.Action<Vector3Int, T> OnDo);
        void Clear(System.Action<Vector3Int, T> OnDo = null);
    }

    public static class IContainerGridExt
    {
        public static bool DefaultIsOutOfBound<T>(this IContainerGrid<T> self, Vector3Int pos)
            => self.Width < 0 || self.Height < 0 ?
                   false :
                   pos.x < 0 || pos.y < 0 || pos.x >= self.Width || pos.y >= self.Height;

    }
}
