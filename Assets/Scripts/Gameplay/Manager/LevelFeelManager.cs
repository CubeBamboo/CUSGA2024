using CbUtils;
using CbUtils.ActionKit;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    }
}
