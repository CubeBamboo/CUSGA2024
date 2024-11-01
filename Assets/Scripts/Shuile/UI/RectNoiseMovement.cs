using UnityEngine;

namespace Shuile.UI
{
    public class RectNoiseMovement : NoiseMovement
    {
        private Vector2 _origin;
        private RectTransform _rectTransform;

        public bool smooth = true;

        protected override void OnStart()
        {
            _rectTransform = GetComponent<RectTransform>();
            _origin = _rectTransform.anchoredPosition;
        }

        protected override void UpdatePosition(Vector2 scaledNoiseValue)
        {
            var end = _origin + scaledNoiseValue;
            _rectTransform.anchoredPosition = smooth
                ? Vector2.Lerp(_rectTransform.anchoredPosition, end, 0.02f)
                : end;
        }
    }
}
