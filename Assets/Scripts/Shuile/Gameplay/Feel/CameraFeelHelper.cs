using CbUtils.Timing;
using DG.Tweening;
using Shuile.Utils;
using System.Threading;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraFeelHelper
    {
        private bool isCameraShake;
        private readonly Camera mCamera;

        public CameraFeelHelper()
        {
            mCamera = Camera.main;
        }

        public void CameraShake(float duration = 0.2f, float strength = 0.1f, CancellationToken token = default)
        {
            if (isCameraShake)
            {
                return;
            }

            isCameraShake = true;
            var initPos = mCamera.transform.position;
            mCamera.DOShakePosition(duration, strength, 10, 90, false).OnComplete(() =>
            {
                mCamera.transform.position = initPos;
            });

            UtilsCommands.SetTimer(duration, () => isCameraShake = false, token);
        }
    }
}
