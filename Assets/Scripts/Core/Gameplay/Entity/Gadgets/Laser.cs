using CbUtils.Event;
using CbUtils.Extension;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Shuile.Core.Framework;
using Shuile.Rhythm;

namespace Shuile
{
    // attach to laser GameObject
    public class Laser : MonoBehaviour, IRhythmable, IEntity
    {
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private float attackStayTime = 0.8f;

        [Tooltip("time calculate will based on MusicRhythmManager.cs if is true")]
        [SerializeField] private bool useRhythmTime = true;

        public static readonly float InTime = 2f;

        float usingInTime;
        float targetScaleX;
        SpriteRenderer mRenderer;
        private LevelTimingManager timingManager;

        public bool SelfEnable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        private void Awake()
        {
            timingManager = this.GetSystem<LevelTimingManager>();
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
                InternalPlay().Forget();
            }
            catch (System.OperationCanceledException)
            {
                // do nothing // TODO: can't catch this exception
                Debug.Log("Laser play canceled");
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
        public async UniTaskVoid InternalPlay()
        {
            FadeInBehave();

            await UniTask.Delay(System.TimeSpan.FromSeconds(usingInTime),
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());

            AttackBehave().Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(attackStayTime),
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());

            FadeOutBehave();
        }

        private void FadeInBehave()
        {
            mRenderer.color = mRenderer.color.With(a: 0f);
            mRenderer.DOFade(0.2f, 0.8f);

            transform.localScale = transform.localScale.With(x: targetScaleX);
            transform.DOScaleX(targetScaleX * 0.1f, usingInTime);
        }

        private async UniTaskVoid AttackBehave()
        {
            transform.DOScaleX(targetScaleX, 0.15f).SetEase(Ease.OutBack);
            mRenderer.DOFade(1f, 0.15f);

            var evtMono = gameObject.GetOrAddComponent<Collider2DEventMono>();
            evtMono.TriggerStayed += (collider) =>
            {
                if (collider.CompareTag("Player"))
                {
                    collider.GetComponent<Player>().OnHurt(200); // TODO: config
                    if (evtMono) evtMono.Destroy(); //销毁组件
                }
            };

            await UniTask.DelayFrame(30, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
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
            usingInTime = InTime;

            if (useRhythmTime)
            {
                usingInTime = this.GetRealTime(InTime, timingManager);
                attackStayTime = this.GetRealTime(attackStayTime, timingManager);
            }
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;

        public void OnInitData(object data)
        {
            throw new System.NotImplementedException();
        }
    }
}
