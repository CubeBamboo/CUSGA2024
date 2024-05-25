using CbUtils.Timing;
using DG.Tweening;
using UnityEngine;

namespace Shuile
{
    public class CameraFeelHelper
    {
        private Camera mCamera;
        private bool isCameraShake;

        public CameraFeelHelper()
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

            TimingCtrl.Instance.Timer(duration, () => isCameraShake = false)
                .Start();
        }
    }
}
