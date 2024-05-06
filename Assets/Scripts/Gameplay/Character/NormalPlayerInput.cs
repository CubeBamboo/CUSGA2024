using CbUtils;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Shuile.Gameplay
{
    /* logic chain: PlayerCtrl -> NormalPlayerInput
     */
    public class NormalPlayerInput : MonoBehaviour
    {
        private Player player;
        private PlayerInput playerInput;

        public float XInput { get; private set; }

        //public event System.Action OnJumpPress, OnJumpHold, OnJumpRelese;
        //public event System.Action<float> OnMove;

        // [for inputHelper]
        public EasyEvent<float> OnMoveStart = new(), OnMoveCanceled = new();
        public EasyEvent<float> OnJumpStart = new(), OnJumpCanceled = new();
        public EasyEvent<float> OnAttackStart = new(), OnAttackCanceled = new();

        PlayerPlayerInputHelper inputHelper = new();

        private void Awake()
        {
            player = GetComponent<Player>();
            playerInput = GetComponent<PlayerInput>();
        }
        private void Start()
        {
            InitInputHelper();
            playerInput.onActionTriggered += inputHelper.OnPlayerInputTriggered;

            player.OnDie.Register(() => this.enabled = false)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private void OnDestroy()
        {
            playerInput.onActionTriggered -= inputHelper.OnPlayerInputTriggered;
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            // jump
            //bool jumpPressing = keyboard.spaceKey.isPressed;
            //if (jumpPressing)
            //{
            //    OnJumpHold?.Invoke();
            //}
            //bool jumpInput = keyboard.spaceKey.wasPressedThisFrame;
            //if (jumpInput)
            //{
            //    OnJumpPress?.Invoke();
            //}
            //bool jumpRelese = keyboard.spaceKey.wasReleasedThisFrame;
            //if (jumpRelese)
            //{
            //    OnJumpRelese?.Invoke();
            //}

            // move
            //bool moveInput = keyboard.aKey.isPressed || keyboard.dKey.isPressed || keyboard.leftArrowKey.isPressed || keyboard.rightArrowKey.isPressed;
            //if (moveInput)
            //{
            //    int dir = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1 : 1;
            //    XInput = dir;
            //    OnMove?.Invoke(dir);
            //}

            // attack
        }

        private void InitInputHelper()
        {
            inputHelper.onMoveStart = v => OnMoveStart.Invoke(v);
            inputHelper.onMoveCanceled = v => OnMoveCanceled.Invoke(v);
            inputHelper.onJumpStart = v => OnJumpStart.Invoke(v);
            inputHelper.onJumpCanceled = v => OnJumpCanceled.Invoke(v);
            inputHelper.onAttackStart = v => OnAttackStart.Invoke(v);
            inputHelper.onAttackCanceled = v => OnAttackCanceled.Invoke(v);
        }
    }

    public class PlayerPlayerInputHelper
    {
        public System.Action<float> onMoveStart, onMoveCanceled;
        public System.Action<float> onJumpStart, onJumpCanceled;
        public System.Action<float> onAttackStart, onAttackCanceled;

        public void OnPlayerInputTriggered(InputAction.CallbackContext ctx)
        {
            switch (ctx.action.name)
            {
                case "HorizontalMove":
                    ProcessMovePhase(ctx);
                    break;
                case "Jump":
                    ProcessJumpPhase(ctx);
                    break;
                case "Fire":
                    ProcessAttackPhase(ctx);
                    break;
            }
        }

        private void InternalProcessPhase<T>(InputAction.CallbackContext ctx, System.Action<T> onStart, System.Action<T> onCanceled)
            where T : struct
        {
            if(onStart == null || onCanceled == null)
            {
                Debug.LogWarning("onStart or onCanceled is null.");
                return;
            }

            switch (ctx.phase)
            {
                case InputActionPhase.Started:
                    onStart.Invoke(ctx.ReadValue<T>());
                    break;
                case InputActionPhase.Canceled:
                    onCanceled.Invoke(ctx.ReadValue<T>());
                    break;
            }
        }

        private void ProcessJumpPhase(InputAction.CallbackContext ctx) => InternalProcessPhase(ctx, onJumpStart, onJumpCanceled);
        private void ProcessMovePhase(InputAction.CallbackContext ctx) => InternalProcessPhase(ctx, onMoveStart, onMoveCanceled);
        private void ProcessAttackPhase(InputAction.CallbackContext ctx) => InternalProcessPhase(ctx, onAttackStart, onAttackCanceled);
    }
}
