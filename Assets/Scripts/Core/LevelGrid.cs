using CbUtils;
using Shuile.Framework;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CbUtils.Collections;

namespace Shuile
{
    public class LevelGrid : MonoSingletons<LevelGrid>
    {
        public ArrayGridContainer<GameObject> grid { get; private set; }
        public Vector2 startPosition = new Vector2(-12, -5);
        private Vector3 cellSize = Vector3.one;
        private Vector3 cellGap = Vector3.zero;
        public int width = 24;
        public int height = 1;

        private HashSet<Vector3Int> validCells = new();
        
        protected override void OnAwake()
        {
            grid = new ArrayGridContainer<GameObject>(startPosition, cellSize, cellGap, width, height);
            MainGame.Interface.Register<LevelGrid>(this);

            InitHashSet();
        }

        //private void Update()
        //{
        //    grid.UpdateConfig(startPosition, cellSize, cellGap, width, height);
        //}

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            grid.OnDrawGizmosSelected();
        }
#endif

        private void OnDestroy()
        {
            MainGame.Interface.UnRegister<LevelGrid>();
        }

        private void InitHashSet()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    validCells.Add(new Vector3Int(i, j, 0));

            grid.OnAdd += (pos, obj) =>
            {
                if (obj == null) return;
                validCells.Remove(pos);
            };
            grid.OnRemove += (pos, obj) =>
            {
                if (obj == null) return;
                validCells.Add(pos);
            };
            grid.OnMove += (from, to) =>
            {
                validCells.Add(from);
                validCells.Remove(to);
            };
        }

        public bool TryGetRandomPosition(out Vector3Int res, bool isValidCell = false)
        {
            res = Vector3Int.zero;
            if (!isValidCell)
            {
                res = new Vector3Int(Random.Range(0, width), Random.Range(0, height));
                return true;
            }

            if(validCells.Count == 0) return false;
            res = validCells.ElementAt(Random.Range(0, validCells.Count));
            return true;
        }
    }
}
