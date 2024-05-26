using CbUtils;
using UnityEngine;

namespace Shuile
{
    // shit for data generate in .scene
    public class LevelDataGetter : MonoNotAutoSpawnSingletons<LevelDataGetter>
    {
        [System.Serializable]
        public struct CameraParallaxMoveCtrlData
        {
            public Transform origin;
            public Transform player;
            public float moveScale; // = 0.1f;
            public float moveSpeed; // = 0.02f;
            public float moveRadius; // = 0.1f;
        }

        public CameraParallaxMoveCtrlData cameraParallaxMove;

        protected override void OnAwake() => OnInitializeData();
        
        private void OnDestroy() => OnDeInitializeData();

        public void OnInitializeData()
        {

        }

        public void OnDeInitializeData()
        {

        }
    }
}
