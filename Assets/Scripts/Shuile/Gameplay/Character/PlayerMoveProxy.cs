using Shuile.Framework;
using Shuile.Gameplay.Move;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl
    {
        private class PlayerMoveProxy : BaseProxy
        {
            private float _moveInputState;

            private SmoothMoveCtrl _moveController;
            private PlayerModel _playerModel;

            public PlayerMoveProxy(UnityEntryPointScheduler scheduler, IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                dependencies
                    .Resolve(out NormalPlayerInput playerInput)
                    .Resolve(out _moveController)
                    .Resolve(out _playerModel);

                playerInput.OnMoveStart.Register(v => _moveInputState = v);
                playerInput.OnMoveCanceled.Register(v => _moveInputState = 0);

                scheduler.AddFixedUpdate(() =>
                {
                    var xInput = _moveInputState;
                    _moveController.XMove(xInput);
                    _playerModel.faceDir = xInput;
                });
            }
        }
    }
}
