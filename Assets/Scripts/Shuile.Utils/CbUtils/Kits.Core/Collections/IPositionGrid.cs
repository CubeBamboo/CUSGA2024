using UnityEngine;

namespace CbUtils.Collections
{
    /// <summary>
    /// provide CellToWorld() and WorldToCell(), it's similar to UnityEngine.GridLayout.
    /// </summary>
    public interface IPositionGrid
    {
        Vector3 OriginPosition { get; }
        Vector3 CellSize { get; }
        Vector3 CellGap { get; }

        public Vector3Int WorldToCell(Vector3 pos)
        {
            pos -= this.OriginPosition;
            return new Vector3Int((int)(pos.x / (this.CellSize.x + this.CellGap.x)), (int)(pos.y / (this.CellSize.y + this.CellGap.y)));
        }

        /// <summary> origin + size + gap + offsetToCentre </summary>
        public Vector3 CellToWorld(Vector3Int pos)
            => this.OriginPosition + new Vector3((this.CellSize.x + this.CellGap.x) * pos.x, (this.CellSize.y + this.CellGap.y) * pos.y) + this.CellSize / 2;
    }

    public static class IPositionGridExt
    {
        public static Vector3Int DefaultWorldToCell(this IPositionGrid self, Vector3 pos) => self.WorldToCell(pos);

        public static Vector3 DefaultCellToWorld(this IPositionGrid self, Vector3Int pos) => self.CellToWorld(pos);

        public static Vector3 SnapToGrid(this IPositionGrid self, Vector3 pos)
            => self.CellToWorld(self.WorldToCell(pos));
    }

}
