using DG.Tweening;
using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.UI
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

        private void Start()
        {
            var lifeTimeLocator = LevelScope.Interface;
            _autoPlayChartManager = lifeTimeLocator.GetImplementation<AutoPlayChartManager>();
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
    }
}
