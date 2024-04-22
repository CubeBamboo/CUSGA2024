using CbUtils.Extension;
using Shuile.Gameplay;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using CbUtils.Event;
using System.Runtime.CompilerServices;

namespace Shuile.NoteProduct
{
    // 起因：方竹懒得写monobehavior
    public static class NoteProductController
    {
        public static class Laser
        {
            private const float attackWaitTime = 0.8f;

            public static async UniTaskVoid Process(GameObject target)
            {
                SpriteRenderer mRenderer = target.GetComponent<SpriteRenderer>();
                target.SetOnDestroy(() => mRenderer.DOKill(), "renderer");

                // 淡入
                mRenderer.color = mRenderer.color.With(a: 0);
                mRenderer.DOFade(0.2f, 0.5f);

                float inTime = 2 * GameplayService.Interface.LevelModel.BpmIntervalInSeconds;
                await UniTask.Delay(System.TimeSpan.FromSeconds(inTime),
                    cancellationToken: target.GetCancellationTokenOnDestroy());
                // 攻击判定
                mRenderer.DOFade(1f, 0.2f);
                var evtMono = target.AddComponent<Collider2DEventMono>();
                evtMono.TriggerStayed += (collider) =>
                {
                    if (collider.CompareTag("Player"))
                    {
                        collider.GetComponent<Player>().OnHurt(200); // TODO: config
                        if(evtMono) evtMono.Destroy(); //销毁组件
                    }
                };
                await UniTask.DelayFrame(2,
                        cancellationToken: target.GetCancellationTokenOnDestroy());
                if (evtMono) evtMono.Destroy(); //销毁组件

                await UniTask.Delay(System.TimeSpan.FromSeconds(attackWaitTime),
                    cancellationToken: target.GetCancellationTokenOnDestroy());
                // 淡出
                mRenderer.DOFade(0, 0.5f).OnComplete(()=>
                    target.Destroy()
                );
            }
        }
    }
}
