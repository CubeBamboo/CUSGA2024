using CbUtils;
using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using System.Collections;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl
    {
        /// <summary>
        ///     state machine based on event driven
        /// </summary>
        private class PlayerJumpProxy : BaseProxy
        {
            private bool _enableUpUpdate;
            private bool _isFalling;
            private bool _isWaitJump;
            private Coroutine _delayStopUpCoroutine;
            private EasyEvent _onFallToGround = new();
            private EasyEvent _onFallStart = new();

            private readonly UnityEntryPointScheduler _scheduler;
            private readonly SmoothMoveCtrl _moveController;
            private Settings _settings;

            private MonoAudioChannel _audioChannel;
            private AudioClip _jumpFx;

            public PlayerJumpProxy(UnityEntryPointScheduler scheduler,
                IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                _scheduler = scheduler;
                dependencies
                    .Resolve(out _moveController)
                    .Resolve(out _audioChannel)
                    .Resolve(out _settings);

                ConfigureEvent();
                _scheduler.AddFixedUpdate(FixedUpdate);

                var resourceLoader = new ResourceLoader();
                _jumpFx = resourceLoader.Load<AudioClip>("Assets/Audio/Test/jump.wav");
            }

            private void FixedUpdate()
            {
                if (_enableUpUpdate)
                {
                    // lifting power
                    _moveController.Velocity += new Vector2(0, _settings.holdJumpVelAdd);
                }

                if (!_moveController.IsOnGround && Mathf.Abs(_moveController.Velocity.y) < 1e-4) // hit wall
                {
                    _enableUpUpdate = false;
                }

                if (_isFalling && _moveController.IsOnGround)
                {
                    _isFalling = false;
                    _onFallToGround.Invoke();
                }
                else if (!_moveController.IsOnGround && _moveController.Velocity.y < -0.1f)
                {
                    _isFalling = true;
                    _onFallStart.Invoke();
                }
            }

            private IEnumerator DelayStopUp()
            {
                yield return new WaitForSeconds(_settings.jumpMaxDuration);
                _enableUpUpdate = false;
            }

            private async UniTask CoolDownJump()
            {
                await UniTask.DelayFrame(10); // shit coroutine no delay frame
                _isWaitJump = false;
            }

            private void ConfigureEvent()
            {
                _settings.onInputJumpStart.Register(OnJumpStart);
                _settings.onInputJumpCanceled.Register(OnJumpCanceled);
                _onFallToGround.Register(() => _moveController.Gravity = _settings.normalGravity);
                _onFallStart.Register(() => _moveController.Gravity = _settings.dropGravity);
                return;

                // 松开
                void OnJumpCanceled(float _)
                {
                    _enableUpUpdate = false;
                    _moveController.Gravity = _settings.dropGravity;
                }

                // 按下
                void OnJumpStart(float _)
                {
                    if (!_isWaitJump && _moveController.IsOnGround)
                    {
                        if (_delayStopUpCoroutine != null)
                        {
                            _scheduler.StopCoroutine(_delayStopUpCoroutine);
                            _delayStopUpCoroutine = null;
                        }

                        _moveController.Velocity = _moveController.Velocity.With(y: _settings.jumpStartVel);
                        _audioChannel.Play(_jumpFx);

                        _enableUpUpdate = true;
                        _delayStopUpCoroutine = _scheduler.StartCoroutine(DelayStopUp());

                        _moveController.Gravity = _settings.normalGravity;

                        _isWaitJump = true;
                        CoolDownJump().Forget();
                    }
                }
            }

            public void RefreshSettings(Settings settings)
            {
                _settings = settings;
            }

            public struct Settings
            {
                public float jumpStartVel;
                public float holdJumpVelAdd;
                public float jumpMaxDuration; // 0.27f
                public float normalGravity;
                public float dropGravity;

                // this is the entry point
                public EasyEvent<float> onInputJumpStart;
                public EasyEvent<float> onInputJumpCanceled;
            }
        }
    }
}
