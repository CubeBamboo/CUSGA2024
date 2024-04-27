using CbUtils.Event;
using CbUtils.Extension;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile
{
    // attach to laser GameObject
    public class Laser : MonoBehaviour, IRhythmable
    {
        private static float inTime = 2f;

        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private float attackStayTime = 0.8f;

        [Tooltip("time calculate will based on MusicRhythmManager.cs if is true")]
        [SerializeField] private bool useRhythmTime = true;

        public static float InTime => inTime;

        float targetScaleX;
        SpriteRenderer mRenderer;

        private void Awake()
        {
            mRenderer = GetComponent<SpriteRenderer>();
            gameObject.SetOnDestroy(() => mRenderer.DOKill(), "renderer");
            targetScaleX = transform.localScale.x;
        }

        private void Start()
        {
            InitParameters();

            if (playOnAwake)
                Play();
        }

        public void Play()
        {
            try
            {
                InternalPlay();
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
        public async void InternalPlay()
        {
            FadeInBehave();

            await UniTask.Delay(System.TimeSpan.FromSeconds(inTime),
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());

            await AttackBehave();

            await UniTask.Delay(System.TimeSpan.FromSeconds(attackStayTime),
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());

            FadeOutBehave();
        }

        private void FadeInBehave()
        {
            mRenderer.color = mRenderer.color.With(a: 0f);
            mRenderer.DOFade(0.2f, 0.8f);

            transform.localScale = transform.localScale.With(x: targetScaleX);
            transform.DOScaleX(targetScaleX * 0.1f, inTime);
        }

        private async UniTask AttackBehave()
        {
            transform.DOScaleX(targetScaleX, 0.15f).SetEase(Ease.OutBack);
            mRenderer.DOFade(1f, 0.15f);

            var evtMono = gameObject.AddComponent<Collider2DEventMono>();
            evtMono.TriggerStayed += (collider) =>
            {
                if (collider.CompareTag("Player"))
                {
                    collider.GetComponent<Player>().OnHurt(200); // TODO: config
                    if (evtMono) evtMono.Destroy(); //销毁组件
                }
            };

            await UniTask.DelayFrame(2, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            if (evtMono) evtMono.Destroy(); //销毁组件
        }

        private void FadeOutBehave()
        {
            mRenderer.DOFade(0, 0.5f).OnComplete(() =>
                gameObject.Destroy()
            );
        }

        private void InitParameters()
        {
            if (useRhythmTime)
            {
                inTime = this.GetRhythmTime(inTime);
                attackStayTime = this.GetRhythmTime(attackStayTime);
            }
        }
    }
}
