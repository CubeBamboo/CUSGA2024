using CbUtils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay.Character
{
    public class NormalPlayerInput : MonoBehaviour
    {
        private readonly PlayerPlayerInputHelper inputHelper = new();
        public EasyEvent<float> OnAttackStart = new(), OnAttackCanceled = new();
        public EasyEvent<float> OnJumpStart = new(), OnJumpCanceled = new();

        // [for inputHelper]
        public EasyEvent<float> OnMoveStart = new(), OnMoveCanceled = new();
        private Player player;
        private PlayerInput playerInput;

        private void Awake()
        {
            player = GetComponent<Player>();
            playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            InitInputHelper();
            playerInput.onActionTriggered += inputHelper.OnPlayerInputTriggered;

            player.OnDie.Register(() => enabled = false)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnDestroy()
        {
            playerInput.onActionTriggered -= inputHelper.OnPlayerInputTriggered;
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

        public void ClearAll()
        {
            inputHelper.ClearAll();
        }
    }

    public class PlayerPlayerInputHelper
    {
        private readonly InternalHelper internalHelper = new();
        public Action<float> onAttackStart, onAttackCanceled;
        public Action<float> onJumpStart, onJumpCanceled;

        public Action<float> onMoveStart, onMoveCanceled;

        public PlayerPlayerInputHelper()
        {
            internalHelper.RegisterActionHandler("HorizontalMove", ProcessMovePhase);
            internalHelper.RegisterActionHandler("Jump", ProcessJumpPhase);
            internalHelper.RegisterActionHandler("Fire", ProcessAttackPhase);
        }

        private void ProcessJumpPhase(InputAction.CallbackContext ctx)
        {
            internalHelper.InternalProcessPhase(ctx, onJumpStart, onJumpCanceled);
        }

        private void ProcessMovePhase(InputAction.CallbackContext ctx)
        {
            internalHelper.InternalProcessPhase(ctx, onMoveStart, onMoveCanceled);
        }

        private void ProcessAttackPhase(InputAction.CallbackContext ctx)
        {
            internalHelper.InternalProcessPhase(ctx, onAttackStart, onAttackCanceled);
        }

        public void OnPlayerInputTriggered(InputAction.CallbackContext ctx)
        {
            internalHelper.OnPlayerInputTriggered(ctx);
        }

        public void ClearAll()
        {
            internalHelper.ClearAll();
        }

        private class InternalHelper
        {
            private readonly Dictionary<string, Action<InputAction.CallbackContext>> actionHandlers = new();

            public void InternalProcessPhase<T>(InputAction.CallbackContext ctx, Action<T> onStart,
                Action<T> onCanceled)
                where T : struct
            {
                if (onStart == null || onCanceled == null)
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

            public bool TryTriggerAction(InputAction.CallbackContext ctx, string actionName,
                Action<InputAction.CallbackContext> handler)
            {
                var res = ctx.action.name == actionName;
                if (res)
                {
                    handler(ctx);
                }

                return res;
            }

            public void RegisterActionHandler(string actionName, Action<InputAction.CallbackContext> handler)
            {
                actionHandlers[actionName] = handler;
            }

            public void UnRegisterActionHandler(string actionName)
            {
                actionHandlers.Remove(actionName);
            }

            public bool OnPlayerInputTriggered(InputAction.CallbackContext ctx)
            {
                var res = actionHandlers.TryGetValue(ctx.action.name, out var handler);
                if (res)
                {
                    handler(ctx);
                }

                return res;
            }

            public void ClearAll()
            {
                actionHandlers.Clear();
            }
        }
    }
}
