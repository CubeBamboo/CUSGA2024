using CbUtils;
using CbUtils.Event;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.NoteProduct
{
    // 起因：方竹懒得写monobehavior
    public static class NoteProductController
    {
        public static class Laser
        {
            /// <summary> in bar </summary>
            private static readonly float inTime = 1.2f;
            private static readonly float attackWaitTime = 0.8f;
            public static float InTime => inTime;

            public static async void Process(GameObject target)
            {
                SpriteRenderer mRenderer = target.GetComponent<SpriteRenderer>();
                var evtMono = target.AddComponent<Collider2DEventMono>();
                evtMono.TriggerEntered += (collider) =>
                {
                    if (collider.CompareTag("Player"))
                    {
                        collider.gameObject.GetComponent<PlayerController>().ForceDie();
                        collider.gameObject.Destroy();
                    }
                }; //TODO: hahaha you forgot to add collider component hahaha
                // 淡入
                mRenderer.color = mRenderer.color.With(a: 0);
                mRenderer.DOFade(0.2f, 0.5f);

                await UniTask.Delay(System.TimeSpan.FromSeconds(inTime));
                // 攻击判定
                mRenderer.DOFade(1f, 0.2f);
                //TODO: attack

                await UniTask.Delay(System.TimeSpan.FromSeconds(attackWaitTime));
                // 淡出
                mRenderer.DOFade(0, 0.5f).OnComplete(()=>
                    target.Destroy()
                );
            }
        }
    }
}
