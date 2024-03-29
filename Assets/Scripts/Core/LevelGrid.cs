using CbUtils;
using Shuile.Framework;
using UnityEngine;

namespace Shuile
{
    public class LevelGrid : MonoSingletons<LevelGrid>
    {
        public ArrayGridContainer<GameObject> grid { get; private set; }
        public Vector2 startPosition; // Vector2(-8, -4)
        public Vector3 cellSize; // Vector3(1, 1, 1)
        public Vector3 cellGap; // Vector3(0, 0, 0)
        public int width; // 16
        public int height; // 1
        
        protected override void OnAwake()
        {
            grid = new ArrayGridContainer<GameObject>(startPosition, cellSize, cellGap, width, height);
            MainGame.Interface.Register<LevelGrid>(this);
        }

        private void OnDrawGizmosSelected()
        {
            //grid.OnDrawGizmosSelected();
        }

        private void OnDestroy()
        {
            MainGame.Interface.UnRegister<LevelGrid>();
        }

        public void GetRandomPosition(out Vector3 res)
        {
            res = new Vector3Int(Random.Range(0, width),
                                 Random.Range(0, height))
                                .ToWorld(grid);
        }

        public void GetRandomPosition(out Vector3Int res)
        {
            res = new Vector3Int(Random.Range(0, width),
                                 Random.Range(0, height));
        }
    }
}
