using UnityEngine;

namespace Shuile
{
    public static class LevelResourcesExtension
    {
        public static GameObject GetParticle(this LevelResources self, string name)
        {
            var particles = self.globalPrefabs.particles;
            foreach (var particle in particles)
                if (particle.name == name)
                    return particle.prefab;
            return null;
        }
    }
}
