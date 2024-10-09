using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl
    {
        private class PlayerMoveProxy : BaseProxy
        {
            private HearableProperty<float> _moveInputState = new();
            private readonly UnityEntryPointScheduler.SchedulerTask _moveUpdate;

            private SmoothMoveCtrl _moveController;
            private PlayerModel _playerModel;
            private EasyEvent<float> OnMoveStart;

            public PlayerMoveProxy(UnityEntryPointScheduler scheduler, IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                dependencies
                    .Resolve(out NormalPlayerInput playerInput)
                    .Resolve(out _moveController)
                    .Resolve(out _playerModel);

                OnMoveStart = playerInput.OnMoveStart;
                playerInput.OnMoveStart.Register(v => _moveInputState.Value = v);
                playerInput.OnMoveCanceled.Register(v => _moveInputState.Value = 0);

                _moveUpdate = scheduler.AddFixedUpdate(() =>
                {
                    NormalMove(_moveInputState.Value);
                });
                _moveUpdate.IsEnabled = false;

                _moveInputState.onValueChanged.Register((_, curr) =>
                {
                    _moveUpdate.IsEnabled = curr != 0;
                });
            }

            /// <summary> update velocity </summary>
            private void NormalMove(float xInput)
            {
                _moveController.XMove(xInput);
                OnMoveStart?.Invoke(xInput);
                _playerModel.faceDir = xInput;
            }
        }
    }
}
