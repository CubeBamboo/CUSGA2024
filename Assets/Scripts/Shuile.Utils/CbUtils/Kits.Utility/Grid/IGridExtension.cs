using CbUtils.Collections;
using UnityEngine;

namespace CbUtils.Extension
{
    public static class IGridExtension
    {
        public static Vector3Int ToCell(this Vector3 self, IPositionGrid grid)
            => grid.WorldToCell(self);

        public static Vector3 ToWorld(this Vector3Int self, IPositionGrid grid)
            => grid.CellToWorld(self);
    }
}
