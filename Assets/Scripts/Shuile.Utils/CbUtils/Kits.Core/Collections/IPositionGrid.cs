using UnityEngine;

namespace CbUtils.Collections
{
    /// <summary>
    ///     provide CellToWorld() and WorldToCell(), it's similar to UnityEngine.GridLayout.
    /// </summary>
    public interface IPositionGrid
    {
        Vector3 OriginPosition { get; }
        Vector3 CellSize { get; }
        Vector3 CellGap { get; }

        public Vector3Int WorldToCell(Vector3 pos)
        {
            pos -= OriginPosition;
            return new Vector3Int((int)(pos.x / (CellSize.x + CellGap.x)), (int)(pos.y / (CellSize.y + CellGap.y)));
        }

        /// <summary> origin + size + gap + offsetToCentre </summary>
        public Vector3 CellToWorld(Vector3Int pos)
        {
            return OriginPosition + new Vector3((CellSize.x + CellGap.x) * pos.x, (CellSize.y + CellGap.y) * pos.y) +
                   (CellSize / 2);
        }
    }

    public static class IPositionGridExt
    {
        public static Vector3Int DefaultWorldToCell(this IPositionGrid self, Vector3 pos)
        {
            return self.WorldToCell(pos);
        }

        public static Vector3 DefaultCellToWorld(this IPositionGrid self, Vector3Int pos)
        {
            return self.CellToWorld(pos);
        }

        public static Vector3 SnapToGrid(this IPositionGrid self, Vector3 pos)
        {
            return self.CellToWorld(self.WorldToCell(pos));
        }
    }
}
