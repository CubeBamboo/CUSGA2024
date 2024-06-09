using CbUtils;
using System;
using UnityEngine;

namespace Shuile.Gameplay
{
    // shit for data generate in .scene
    public class LevelDataGetter : MonoNotAutoSpawnSingletons<LevelDataGetter>
    {
        public CameraParallaxMoveCtrlData cameraParallaxMove;

        private void OnDestroy()
        {
            OnDeInitializeData();
        }

        protected override void OnAwake()
        {
            OnInitializeData();
        }

        public void OnInitializeData()
        {
        }

        public void OnDeInitializeData()
        {
        }

        [Serializable]
        public struct CameraParallaxMoveCtrlData
        {
            public Transform origin;
            public float moveScale; // = 0.1f;
            public float moveSpeed; // = 0.02f;
            public float moveRadius; // = 0.1f;
        }
    }
}
