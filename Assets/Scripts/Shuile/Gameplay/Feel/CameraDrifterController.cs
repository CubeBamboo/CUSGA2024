using Shuile.Framework;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterController : MonoBehaviour
    {
        private Transform _player;

        public bool HasTargetPosition => _player != null;
        public Vector2 TargetPosition => _player.position;

        private void Start()
        {
            // GameplayScene can be inherited so it can be reusable.
            if (((GamePlayScene)ContainerExtensions.FindSceneContainer()).TryGetPlayer(out var player))
            {
                _player = player.transform;
            }
        }
    }
}
