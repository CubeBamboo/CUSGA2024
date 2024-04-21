using CbUtils;
using Shuile.Gameplay;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using CbUtils.Event;

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

                float inTime = 2 * GameplayService.LevelModel.Value.BpmIntervalInSeconds;
                await UniTask.Delay(System.TimeSpan.FromSeconds(inTime),
                    cancellationToken: target.GetCancellationTokenOnDestroy());
                // 攻击判定
                mRenderer.DOFade(1f, 0.2f);
                //var evtMono = target.AddComponent<Collider2DEventMono>();
                //evtMono.TriggerEntered += (collider) =>
                //{
                //    if (collider.CompareTag("Player"))
                //    {
                //        collider.gameObject.GetComponent<Player>().ForceDie();
                //        collider.gameObject.Destroy();
                //    }
                //}; //TODO: hahaha you forgot to add collider component hahaha

                await UniTask.Delay(System.TimeSpan.FromSeconds(attackWaitTime));
                // 淡出
                mRenderer.DOFade(0, 0.5f).OnComplete(()=>
                    target.Destroy()
                );
            }
        }
    }
}
