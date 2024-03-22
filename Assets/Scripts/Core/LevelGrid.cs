using CbUtils;
using Shuile.Framework;
using System.Collections;
using System.Collections.Generic;
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
            grid = new(startPosition, cellSize, cellGap, width, height);
            MainGame.Interface.Register<LevelGrid>(this);
        }

        private void OnDrawGizmosSelected()
        {
            grid.OnDrawGizmosSelected();
        }

        private void OnDestroy()
        {
            MainGame.Interface.UnRegister<LevelGrid>();
        }
    }
}
