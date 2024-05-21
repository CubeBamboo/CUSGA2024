using CbUtils;
using CbUtils.Extension;

using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// manage all about game feeling (like camera shake, particle effects, sound effects)
    /// </summary>
    public class LevelFeelManager : MonoSingletons<LevelFeelManager>
    {
        CameraFeelHelper cameraFeelHelper;
        VolumeFeelHelper volumeFeelHelper;
        private MonoLevelResources levelResources => MonoLevelResources.Instance;

        protected override void OnAwake()
        {
            cameraFeelHelper = new();
            volumeFeelHelper = new();
        }

        public void CameraShake(float duration = 0.2f, float strength = 0.1f)
            => cameraFeelHelper.CameraShake(duration, strength);
        
        /// <param name="name"> see in LevelResources.particles </param>
        public void PlayParticle(string name, Vector3 position, Vector3 direction, Transform parent = null)
            => LevelFeelFactory.CreateParticle(name, position, direction).SetParent(parent);

        public void VignettePulse() => volumeFeelHelper.VignettePulse();
        public void VignettePulse(Color targetColor, float lerpValue = 0.3f)
            => volumeFeelHelper.VignettePulse(targetColor, lerpValue);
    }
}
