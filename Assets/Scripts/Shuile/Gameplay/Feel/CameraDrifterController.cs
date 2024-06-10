using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Character;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterController : MonoBehaviour, IEntity
    {
        private Transform _player;

        public bool HasTargetPosition => _player != null;
        public Vector2 TargetPosition => _player.position;

        private void Start()
        {
            var scope = LevelScope.Interface;
            var player = scope.GetImplementation<Player>();
            this._player = player.transform;
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
