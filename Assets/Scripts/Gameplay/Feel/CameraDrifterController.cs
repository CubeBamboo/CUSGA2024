using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Event;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterController : MonoEntity
    {
        private CameraDrifter _cameraDrifter;
        private Transform player;

        private void Start()
        {
            _cameraDrifter = GetComponent<CameraDrifter>();

            var data = LevelDataGetter.Instance.cameraParallaxMove;
            player = data.player;
            _cameraDrifter.originPosition = data.origin.position;
            _cameraDrifter.moveScale = data.moveScale;
            //_cameraDrifter.moveSpeed = data.moveSpeed;
            _cameraDrifter.moveRadius = data.moveRadius;
        }

        private void LateUpdate()
        {
            if (player == null) return;
            _cameraDrifter.TargetPosition = _cameraDrifter.GetUsingTargetValue(player.position);
        }

        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
