using CbUtils;
using CbUtils.ActionKit;
using CbUtils.Extension;
using DG.Tweening;
using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// manage all about game feeling (like camera shake, particle effects, sound effects)
    /// </summary>
    public class LevelFeelManager : MonoSingletons<LevelFeelManager>
    {
        private Camera mCamera;
        private bool isCameraShake;
        private LevelResources levelResources => LevelResources.Instance;

        protected override void OnAwake()
        {
            mCamera = Camera.main;
        }

        public void CameraShake(float duration = 0.2f, float strength = 0.1f)
        {
            if (isCameraShake) return;

            isCameraShake = true;
            var initPos = mCamera.transform.position;
            mCamera.DOShakePosition(duration, strength, 10, 90, false).OnComplete(() =>
            {
                mCamera.transform.position = initPos;
            });
            ActionCtrl.Delay(0.2f)
                .OnComplete(() => isCameraShake = false)
                .Start(mCamera.gameObject);
        }
        
        /// <param name="name"> see in LevelResources.particles </param>
        public void PlayParticle(string name, Vector3 position, Vector3 direction, Transform parent = null)
            => LevelFeelFactory.CreateParticle(name, position, direction).SetParent(parent);
    }
}
