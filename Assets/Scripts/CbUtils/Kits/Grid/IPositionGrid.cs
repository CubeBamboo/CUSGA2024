using UnityEngine;

namespace CbUtils
{
    /// <summary>
    /// provide CellToWorld() and WorldToCell(), it's similar to UnityEngine.GridLayout.
    /// </summary>
    public interface IPositionGrid
    {
        Vector3 OriginPosition { get; }
        Vector3 CellSize { get; }
        Vector3 CellGap { get; }

        /// <summary> recommend to use this.DefaultWorldToCell() </summary>
        public Vector3Int WorldToCell(Vector3 pos);
        /// <summary> recommend to use this.DefaultCellToWorld() </summary>
        public Vector3 CellToWorld(Vector3Int pos);
    }

    public static class IPositionGridExt
    {
        public static Vector3Int DefaultWorldToCell(this IPositionGrid self, Vector3 pos)
        {
            pos -= self.OriginPosition;
            return new Vector3Int((int)(pos.x / (self.CellSize.x + self.CellGap.x)), (int)(pos.y / (self.CellSize.y + self.CellGap.y)));
        }

        // origin + size + gap + offsetToCentre
        public static Vector3 DefaultCellToWorld(this IPositionGrid self, Vector3Int pos)
            => self.OriginPosition + new Vector3((self.CellSize.x + self.CellGap.x) * pos.x, (self.CellSize.y + self.CellGap.y) * pos.y) + self.CellSize / 2;

        public static Vector3 SnapToGrid(this IPositionGrid self, Vector3 pos)
            => self.CellToWorld(self.WorldToCell(pos));

    }
}
