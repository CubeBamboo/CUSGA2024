#if UNITY_EDITOR
using CbUtils;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Shuile.Rhythm
{
    public static class TestExt
    {
        public static async void DelayDestroy(float time, GameObject obj, System.Action OnDestroy=null)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(time));
            obj.transform.DOScale(0f, 0.3f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                OnDestroy?.Invoke();
                obj.Destroy();
            });
        }
    }
}
#endif
