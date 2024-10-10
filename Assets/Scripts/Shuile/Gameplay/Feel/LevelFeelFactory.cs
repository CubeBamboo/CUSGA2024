using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public static class LevelFeelFactory
    {
        public static GameObject CreateParticle(string name, Vector3 position, Vector3 direction,
            Transform parent = null)
        {
            var particle = GameApplication.BuiltInData.globalPrefabs.GetParticle(name);
            if (particle == null)
            {
                return null;
            }

            var instance = Object.Instantiate(particle, position, Quaternion.identity);
            instance.transform.SetParent(parent);

            var instanceTransform = instance.transform;

            // initialize
            switch (name)
            {
                case "SwordSlash":
                    var offsetY = 30f;
                    instanceTransform.rotation = Quaternion.Euler(-90f, 0f, 0f) * Quaternion.Euler(0,
                        offsetY + Vector3.SignedAngle(instanceTransform.right, direction, -instanceTransform.forward),
                        0);
                    break;
            }

            return instance;
        }
    }
}
