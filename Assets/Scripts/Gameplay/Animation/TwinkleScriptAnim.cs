using CbUtils;
using CbUtils.ActionKit;
using CbUtils.Event;
using DG.Tweening;
using UnityEngine;

namespace Shuile
{
    public class TwinkleScriptAnim : MonoScriptAnimation
    {
        public enum EndType
        {
            Destroy,
            Disable,
            DoNothing
        }

        private SpriteRenderer _renderer;
        public float fadeOutDuration = 1.0f;
        public float duration = 5.0f;
        public EndType endType = EndType.Destroy;
        public bool waitUtilFadeOut = true;
        protected override void OnAwake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
        protected override void OnPlayAnimation()
        {
            var targetAlpha = _renderer.color.a;
            _renderer.color = _renderer.color.With(a : 0);
            _renderer.DOFade(targetAlpha, fadeOutDuration).SetLoops(-1, LoopType.Yoyo);

            var unit = 2 * fadeOutDuration;
            var waitTime = !waitUtilFadeOut ?
                duration : ((int)(duration / unit) + 1) * unit;
            ActionCtrl.Instance.Delay(waitTime)
                .OnComplete(HandleEnd)
                .Start(gameObject);
        }

        protected override void StopAnimation()
        {
            _renderer.DOKill();
        }

        private void HandleEnd()
        {
            switch (endType)
            {
                case EndType.Destroy:
                    gameObject.Destroy();
                    break;
                case EndType.Disable:
                    gameObject.SetActive(false);
                    break;
                case EndType.DoNothing:
                    break;
            }
        }
    }
}
