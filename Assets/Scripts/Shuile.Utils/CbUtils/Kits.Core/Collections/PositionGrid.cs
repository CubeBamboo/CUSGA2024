using UnityEngine;

namespace CbUtils.Collections
{
    public class GlobalPositionGrid : IPositionGrid
    {
        private static readonly GlobalPositionGrid instance = new();

        public Vector3 OriginPosition { get; private set; } = Vector3.zero;

        public Vector3 CellSize { get; private set; } = Vector3.one;

        public Vector3 CellGap { get; private set; } = Vector3.zero;

        public static void SetValue(Vector3? originPosition = null, Vector3? cellSize = null, Vector3? cellGap = null)
        {
            if (originPosition != null)
            {
                instance.OriginPosition = originPosition.Value;
            }

            if (cellSize != null)
            {
                instance.CellSize = cellSize.Value;
            }

            if (cellGap != null)
            {
                instance.CellGap = cellGap.Value;
            }
        }

        public static Vector3Int WorldToCell(Vector3 pos)
        {
            return instance.DefaultWorldToCell(pos);
        }

        public static Vector3 CellToWorld(Vector3Int pos)
        {
            return instance.DefaultCellToWorld(pos);
        }
    }
}
