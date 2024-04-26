using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;
using static UnityEngine.GraphicsBuffer;

namespace Shuile
{
    public class LevelFeelFactory
    {
        public static void CreateParticle(string name, Vector3 position, Vector3 direction)
        {
            var particle = LevelResources.Instance.GetParticle(name);
            if (particle == null) return;

            var instance = Object.Instantiate(particle, position, Quaternion.identity);
            instance.transform.SetParent(LevelFeelManager.Instance.transform);

            var instanceTransform = instance.transform;

            // initialize
            switch (name)
            {
                case "SwordSlash":
                    var offsetY = 30f;
                    instanceTransform.rotation = Quaternion.Euler(-90f, 0f, 0f) * Quaternion.Euler(0, offsetY + Vector3.SignedAngle(instanceTransform.right, direction, -instanceTransform.forward), 0);
                    break;
                default:
                    break;
            }
        }
    }
}
