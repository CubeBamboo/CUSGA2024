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
            private Rigidbody2D _rb;
            // private readonly SmoothMoveCtrl _moveController;
            private Settings _settings;

            private MonoAudioChannel _audioChannel;
            private AudioClip _jumpFx;

            public PlayerJumpProxy(UnityEntryPointScheduler scheduler,
                IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                _scheduler = scheduler;
                dependencies
                    .Resolve(out GameObject gameObject)
                    .Resolve(out _audioChannel)
                    .Resolve(out _settings);

                ConfigureEvent();
                _scheduler.AddFixedUpdate(FixedUpdate);
                _rb = gameObject.GetComponent<Rigidbody2D>();

                var resourceLoader = new ResourceLoader();
                _jumpFx = resourceLoader.Load<AudioClip>("Assets/Audio/Test/jump.wav");
            }

            private bool IsOnGround => Mathf.Abs(_rb.velocity.y) < 1e-4 && _rb.attachedColliderCount > 0;

            private void FixedUpdate()
            {
                if (_enableUpUpdate)
                {
                    // lifting power
                    _rb.velocity += new Vector2(0, _settings.holdJumpVelAdd);
                }

                if (!IsOnGround && Mathf.Abs(_rb.velocity.y) < 1e-4) // hit wall
                {
                    _enableUpUpdate = false;
                }

                if (_isFalling && IsOnGround)
                {
                    _isFalling = false;
                    _onFallToGround.Invoke();
                }
                else if (!IsOnGround && _rb.velocity.y < -0.1f)
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
                _onFallToGround.Register(() => _rb.gravityScale = _settings.normalGravity);
                _onFallStart.Register(() => _rb.gravityScale = _settings.dropGravity);
                return;

                // 松开
                void OnJumpCanceled(float _)
                {
                    _enableUpUpdate = false;
                    _rb.gravityScale = _settings.dropGravity;
                }

                // 按下
                void OnJumpStart(float _)
                {
                    if (!_isWaitJump && IsOnGround)
                    {
                        if (_delayStopUpCoroutine != null)
                        {
                            _scheduler.StopCoroutine(_delayStopUpCoroutine);
                            _delayStopUpCoroutine = null;
                        }

                        _rb.velocity = _rb.velocity.With(y: _settings.jumpStartVel);
                        _audioChannel.Play(_jumpFx);

                        _enableUpUpdate = true;
                        _delayStopUpCoroutine = _scheduler.StartCoroutine(DelayStopUp());

                        _rb.gravityScale = _settings.normalGravity;

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
