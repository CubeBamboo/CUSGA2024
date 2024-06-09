using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Character;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterController : MonoBehaviour, IEntity
    {
        private Transform player;

        public bool HasTargetPosition => player != null;
        public Vector2 TargetPosition => player.position;

        private void Start()
        {
            player = Player.Instance.transform;
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
