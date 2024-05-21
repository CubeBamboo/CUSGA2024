using Shuile.ResourcesManagement.Loader;
using System.Threading.Tasks;
using UnityEngine;

namespace Shuile
{
    public static class LevelResourcesExtension
    {
        public static GameObject GetParticle(this MonoLevelResources self, string name)
        {
            var particles = self.globalPrefabs.particles;
            foreach (var particle in particles)
                if (particle.name == name)
                    return particle.prefab;
            return null;
        }
        public static async Task<GameObject> GetParticleAsync(this LevelResourcesLoader self, string name)
        {
            var prefabs = await self.GetGlobalPrefabsAsync();
            var particles = prefabs.particles;
            foreach (var particle in particles)
                if (particle.name == name)
                    return particle.prefab;
            return null;
        }
        public static GameObject GetParticle(this LevelResourcesLoader self, string name, PrefabConfigSO prefabs)
        {
            var particles = prefabs.particles;
            foreach (var particle in particles)
                if (particle.name == name)
                    return particle.prefab;
            return null;
        }
    }
}
