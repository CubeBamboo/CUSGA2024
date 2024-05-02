using DG.Tweening;
using Shuile.Rhythm.Runtime;

using System;
using UnityEngine;

namespace Shuile
{
    [RequireComponent(typeof(RectTransform))]
    public class RhythmShakeAnim : MonoBehaviour
    {
        RectTransform rectTransform;
        private Vector3 InitScale;

        [SerializeField] private float shakeScale = 1.2f;
        [SerializeField] private float shakeDuration = 0.2f;

        private float halfShakeDurationCache;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            InitScale = rectTransform.localScale;
            halfShakeDurationCache = shakeDuration / 2;
            AutoPlayChartManager.Instance.OnRhythmHit += OnRhythmHit;
        }

        private void OnDestroy()
        {
            rectTransform.DOKill();
            AutoPlayChartManager.Instance.OnRhythmHit -= OnRhythmHit;
        }

        private void OnRhythmHit()
        {
            rectTransform.DOScale(InitScale * shakeScale, halfShakeDurationCache).OnComplete(() =>
            {
                rectTransform.DOScale(InitScale, halfShakeDurationCache);
            });
        }
    }
}
