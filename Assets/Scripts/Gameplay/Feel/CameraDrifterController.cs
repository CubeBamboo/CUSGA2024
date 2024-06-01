using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterController : MonoEntity
    {
        private Transform player;

        public bool HasTargetPosition => player != null;
        public Vector2 TargetPosition => player.position;

        private void Start()
        {
            player = Player.Instance.transform;
        }

        public override ModuleContainer GetModule() => GameApplication.Level;
    }
}
