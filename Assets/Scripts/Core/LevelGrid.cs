using CbUtils;
using Shuile.Framework;
using UnityEngine;

namespace Shuile
{
    public class LevelGrid : MonoSingletons<LevelGrid>
    {
        public ArrayGridContainer<GameObject> grid { get; private set; }
        public Vector2 startPosition;
        public Vector3 cellSize;
        public Vector3 cellGap;
        public int width;
        public int height;
        
        protected override void OnAwake()
        {
            grid = new ArrayGridContainer<GameObject>(startPosition, cellSize, cellGap, width, height); //TODO: positiongrid
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
