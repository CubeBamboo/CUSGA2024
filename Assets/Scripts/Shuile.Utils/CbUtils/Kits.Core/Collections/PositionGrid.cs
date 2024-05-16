using UnityEngine;

namespace CbUtils.Collections
{
    public class GlobalPositionGrid : IPositionGrid
    {
        private static GlobalPositionGrid instance = new();

        private Vector3 originPosition = Vector3.zero;
        private Vector3 cellSize = Vector3.one;
        private Vector3 cellGap = Vector3.zero;

        public static void SetValue(Vector3? originPosition = null, Vector3? cellSize = null, Vector3? cellGap = null)
        {
            if(originPosition != null) instance.originPosition = originPosition.Value;
            if (cellSize != null) instance.cellSize = cellSize.Value;
            if (cellGap != null) instance.cellGap = cellGap.Value;
        }

        public static Vector3Int WorldToCell(Vector3 pos) => instance.DefaultWorldToCell(pos);
        public static Vector3 CellToWorld(Vector3Int pos) => instance.DefaultCellToWorld(pos);

        public Vector3 OriginPosition => originPosition;
        public Vector3 CellSize => cellSize;
        public Vector3 CellGap => cellGap;
    }

}
