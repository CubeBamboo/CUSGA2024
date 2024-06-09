using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Shuile.Gameplay.Feel
{
    public class VolumeFeelHelper
    {
        Volume _volume;
        private Vignette _vignette;

        public float PulseDuration = 0.3f;
        private float pulseInitIntensity;
        private Color pulseInitColor;

        private const float endIntensity = 0.7f;

        public VolumeFeelHelper()
        {
            _volume = GameObject.FindObjectOfType<Volume>();
            _volume.profile.TryGet<Vignette>(out _vignette);

            pulseInitIntensity = _vignette.intensity.value;
            pulseInitColor = _vignette.color.value;
        }

        public void VignettePulse() => VignettePulse(Color.red);
        public void VignettePulse(Color targetColor, float lerpValue = 0.3f)
        {
            _volume.DOKill();

            targetColor = Color.Lerp(_vignette.color.value, targetColor, lerpValue);

            DOTween.To(() => _vignette.color.value, v => _vignette.color.value = v, targetColor, PulseDuration)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _vignette.color.value = pulseInitColor)
                .SetTarget(_volume)
                .SetLink(_volume.gameObject); // ...
            _vignette.intensity.value = endIntensity;
            DOTween.To(() => _vignette.intensity.value, v => _vignette.intensity.value = v, pulseInitIntensity, PulseDuration)
                .SetTarget(_volume)
                .SetLink(_volume.gameObject);
        }
    }
}
