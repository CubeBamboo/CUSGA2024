using DG.Tweening;
using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

using System;
using UnityEngine;

namespace Shuile
{
    [RequireComponent(typeof(RectTransform))]
    public class RhythmShakeAnim : MonoBehaviour, IEntity
    {
        private AutoPlayChartManager _autoPlayChartManager;

        RectTransform rectTransform;
        private Vector3 InitScale;

        [SerializeField] private float shakeScale = 1.2f;
        [SerializeField] private float shakeDuration = 0.2f;

        private float halfShakeDurationCache;

        public bool SelfEnable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void Start()
        {
            _autoPlayChartManager = this.GetSystem<AutoPlayChartManager>();
            rectTransform = GetComponent<RectTransform>();

            InitScale = rectTransform.localScale;
            halfShakeDurationCache = shakeDuration / 2;
            _autoPlayChartManager.OnRhythmHit += OnRhythmHit;
        }

        private void OnDestroy()
        {
            rectTransform.DOKill();
            _autoPlayChartManager.OnRhythmHit -= OnRhythmHit;
        }

        private void OnRhythmHit()
        {
            rectTransform.DOScale(InitScale * shakeScale, halfShakeDurationCache).OnComplete(() =>
            {
                rectTransform.DOScale(InitScale, halfShakeDurationCache);
            });
        }

        public ModuleContainer GetModule() => GameApplication.Level;

        public void OnInitData(object data)
        {
            throw new NotImplementedException();
        }
    }
}
