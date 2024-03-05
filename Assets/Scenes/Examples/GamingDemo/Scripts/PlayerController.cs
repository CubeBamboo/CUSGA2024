using CbUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CUSGA2024
{
    public class BlockClearHelper
    {
        private MainGridContainer grid;
        private HashSet<Vector3Int> blockToClearFlag = new(); //有则表示方块不要清除

        public BlockClearHelper(MainGridContainer grid)
        {
            this.grid = grid;
        }

        //可能稍微有点复杂
        public void CheckBlockClear()
        {
            blockToClearFlag.Clear();
            //遍历字典所有方块，在地面则开始dfs，遇到bool is true || 周围为空则结束
            foreach (var item in grid.contents)
            {
                //blockToClearFlag.Add(item.Key);
                //在地面
                if (item.Key.y == 0)
                {
                    CheckBlockClearHelper(item.Key);
                }
            }

            foreach(var toClear in blockToClearFlag)
            {
                grid.RemoveContent(toClear, go => Object.Destroy(go));
            }
        }

        private void CheckBlockClearHelper(Vector3Int blockPosition, int depth=0)
        {
            if (depth >= 200)
            {
                Debug.LogWarning("recursion is too deep!"); //孩子有点害怕((
                return;
            }

            if (!grid.HasContent(blockPosition))
                return;

            if (blockToClearFlag.Contains(blockPosition))
                return;

            blockToClearFlag.Remove(blockPosition);
            Debug.Log($"remove block in {blockPosition}");

            depth++;
            //搜索上下左右
            var up = blockPosition + Vector3Int.up;
            var down = blockPosition + Vector3Int.down;
            var left = blockPosition + Vector3Int.left;
            var right = blockPosition + Vector3Int.right;
            CheckBlockClearHelper(up, depth);
            CheckBlockClearHelper(down, depth);
            CheckBlockClearHelper(left, depth);
            CheckBlockClearHelper(right, depth);
        }
    }

    //提供接口来控制两名玩家的逻辑
    [RequireComponent(typeof(MainGridContainer))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer blockPrefab;
        [SerializeField] private Color blockColor;
        //[SerializeField] private Color player2Color;

        [SerializeField] private Transform blockParent;

        public MainGridContainer grid { get; private set; }

        //for block clear check
        private BlockClearHelper blockClearHelper;

        private void Awake()
        {
            grid = GetComponent<MainGridContainer>();
        }

        private void Start()
        {
            blockClearHelper = new BlockClearHelper(grid);
        }

        /// <param name="pos">pos in grid</param>
        public void PlaceBlockWithoutCheckClear(Vector3Int pos)
        {
            if (grid.IsOutOfBound(pos)) return;

            var go = Instantiate(blockPrefab, grid.CellToWorld(pos), Quaternion.identity);
            go.transform.SetParent(blockParent);
            go.color = blockColor;
            grid.AddOrModifyContent(pos, go.gameObject, go => Destroy(go));
        }

        /// <param name="pos">pos in grid</param>
        public void PlaceBlock(Vector3Int pos)
        {
            PlaceBlockWithoutCheckClear(pos);

            blockClearHelper.CheckBlockClear();
        }

        /// <param name="pos">pos in grid</param>
        public void RemoveBlock(Vector3Int pos)
        {
            Destroy(grid.GetContent(pos));
            grid.RemoveContent(pos);

            blockClearHelper.CheckBlockClear();
        }
    }
}
