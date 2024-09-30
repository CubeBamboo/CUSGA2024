using DG.Tweening;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RhythmShakeAnim : MonoBehaviour
    {
        [SerializeField] private float shakeScale = 1.2f;
        [SerializeField] private float shakeDuration = 0.2f;
        private AutoPlayChartManager _autoPlayChartManager;

        private float halfShakeDurationCache;
        private Vector3 InitScale;

        private RectTransform rectTransform;

        private void Start()
        {
            SceneContainer.Instance.Context.ServiceLocator
                .Resolve(out _autoPlayChartManager);

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
    }
}
