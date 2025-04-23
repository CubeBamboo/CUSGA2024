using Shuile.Framework;
using Shuile.Gameplay.Move;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl
    {
        private class PlayerMoveProxy : BaseProxy
        {
            private readonly AnimationCurve _accelerationCurve;
            private readonly AnimationCurve _deAccelerationCurve;
            private int _moveInputState;
            private MoveSettings _moveSettings;
            private GameObject _gameObject;

            private Rigidbody2D _rigidbody2D;

            private SmoothMoveCtrl _moveController;
            private PlayerModel _playerModel;
            private AnimationCurve _moveCurve;

            private float _inputXSmooth;
            private float _inputXVel;

            public float MaxSpeed;

            public PlayerMoveProxy(UnityEntryPointScheduler scheduler, IReadOnlyServiceLocator dependencies, AnimationCurve accelerationCurve, AnimationCurve deAccelerationCurve) : base(scheduler, dependencies)
            {
                _accelerationCurve = accelerationCurve;
                _deAccelerationCurve = deAccelerationCurve;
                dependencies
                    .Resolve(out NormalPlayerInput playerInput)
                    .Resolve(out _moveSettings)
                    .Resolve(out _gameObject)
                    .Resolve(out _playerModel);

                _rigidbody2D = _gameObject.GetComponent<Rigidbody2D>();

                playerInput.OnMoveStart.Register(v => _moveInputState = v > 0 ? 1 : -1);
                playerInput.OnMoveCanceled.Register(v => _moveInputState = 0);

                scheduler.AddFixedUpdate(() =>
                {
                    _inputXSmooth = Mathf.SmoothDamp(_inputXSmooth, _moveInputState, ref _inputXVel, 0.1f);

                    float speed = 0f;
                    if (_moveInputState == 0)
                    {
                        speed = Mathf.Sign(_inputXSmooth) * _deAccelerationCurve.Evaluate(Mathf.Abs(_inputXSmooth)) * _moveSettings.xMaxSpeed;
                    }
                    else
                    {
                        speed = Mathf.Sign(_inputXSmooth) * _accelerationCurve.Evaluate(Mathf.Abs(_inputXSmooth)) * _moveSettings.xMaxSpeed;
                    }
                    _rigidbody2D.velocity = new Vector2(speed, _rigidbody2D.velocity.y);
                    if (_moveInputState != 0) _playerModel.faceDir = _moveInputState;
                });

                // scheduler.AddOnGUI(() =>
                // {
                //     GUI.skin.label.fontSize = 20;
                //     GUILayout.Label($"_moveInputState: {_moveInputState}");
                //     GUILayout.Label($"_inputXSmooth: {_inputXSmooth}");
                //     GUILayout.Label($"_inputXSmooth ***: {_inputXSmooth * _moveSettings.xMaxSpeed}");
                // });
            }
        }
    }
}
