using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    /// <summary>
    ///     manage all about game feeling (like camera shake, particle effects, sound effects)
    /// </summary>
    public class LevelFeelManager
    {
        private readonly CameraFeelHelper _cameraFeelHelper = new();
        private readonly VolumeFeelHelper _volumeFeelHelper = new();

        public void CameraShake(float duration = 0.2f, float strength = 0.1f)
        {
            _cameraFeelHelper.CameraShake(duration, strength);
        }

        public void PlayParticle(string name, Vector3 position, Vector3 direction, Transform parent = null)
        {
            LevelFeelFactory.CreateParticle(name, position, direction, parent);
        }

        public void VignettePulse()
        {
            _volumeFeelHelper.VignettePulse();
        }

        public void VignettePulse(Color targetColor, float lerpValue = 0.3f)
        {
            _volumeFeelHelper.VignettePulse(targetColor, lerpValue);
        }
    }
}
