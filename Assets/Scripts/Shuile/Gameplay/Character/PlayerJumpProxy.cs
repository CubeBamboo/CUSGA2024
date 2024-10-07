﻿using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl
    {
        /// <summary>
        ///     state machine based on event driven
        /// </summary>
        private partial class PlayerJumpProxy : BaseProxy
        {
            private readonly UnityEntryPointScheduler _scheduler;
            private readonly SmoothMoveCtrl _moveController;
            private readonly Settings _settings;

            private HearableProperty<JumpingState> _jumpingState = new();
            private HearableProperty<InputState> _inputState = new(InputState.HoldOff);
            private EasyEvent _onJumpingUpdate = new();
            private EasyEvent _onTouchGround = new();

            private float holdStartTime;

            private UnityEntryPointScheduler.SchedulerTask _jumpingUpdate;
            private UnityEntryPointScheduler.SchedulerTask _fallUpdate;

            public PlayerJumpProxy(UnityEntryPointScheduler scheduler,
                IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                _scheduler = scheduler;
                dependencies
                    .Resolve(out _moveController)
                    .Resolve(out _settings);

                ConfigureBaseEvent();
                ConfigureFacadeEvent();
            }

            private void ConfigureFacadeEvent()
            {
                // jump-enter
                _jumpingState.BindValueChangeTo(JumpingState.JumpUp, () =>
                {
                    _moveController.Velocity = _moveController.Velocity.With(y: _settings.jumpStartVel);

                    // timer
                    holdStartTime = Time.time;
                });

                // jumping
                _onJumpingUpdate.Register(() =>
                {
                    // lifting power
                    _moveController.Velocity += new Vector2(0, _settings.holdJumpVelAdd);

                    var hitWall = Mathf.Abs(_moveController.Velocity.y) < 1e-4; // ...

                    if (Time.time - holdStartTime > _settings.jumpMaxDuration || hitWall || _inputState.Value == InputState.HoldOff)
                    {
                        // force fall
                        _jumpingState.Value = JumpingState.Fall;
                    }
                });

                // fall
                _jumpingState.BindValueChangeTo(JumpingState.Fall, () =>
                {
                    _moveController.Gravity = _settings.dropGravity;
                    _fallUpdate.IsEnabled = true;
                });

                // touch ground
                _onTouchGround.Register(() =>
                {
                    _moveController.Gravity = _settings.normalGravity;
                });
            }

            private void ConfigureBaseEvent()
            {
                _settings.onInputJumpStart.Register(_ =>
                {
                    _inputState.Value = InputState.HoldOn;
                });
                _settings.onInputJumpCanceled.Register(_ =>
                {
                    _inputState.Value = InputState.HoldOff;
                });

                _jumpingUpdate = _scheduler.AddFixedUpdate(() =>
                {
                    _onJumpingUpdate.Invoke();
                });
                _jumpingUpdate.IsEnabled = false;

                _fallUpdate = _scheduler.AddFixedUpdate(() =>
                {
                    if (_moveController.IsOnGround)
                    {
                        _onTouchGround.Invoke();
                        _jumpingState.Value = JumpingState.Idle;
                    }
                });
                _fallUpdate.IsEnabled = false;

                _inputState.BindValueChangeTo(InputState.HoldOn, () =>
                {
                    if (_jumpingState.Value != JumpingState.Idle) return;
                    _jumpingState.Value = JumpingState.JumpUp;
                });

                _jumpingState.BindValueChangeTo(JumpingState.JumpUp, () =>
                {
                    _jumpingUpdate.IsEnabled = true;
                });

                _jumpingState.BindValueChangeTo(JumpingState.Fall, () =>
                {
                    _jumpingUpdate.IsEnabled = false;
                });
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

            private enum JumpingState
            {
                Idle, JumpUp, Fall
            }

            private enum InputState
            {
                HoldOn, HoldOff
            }
        }
    }
}