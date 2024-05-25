using CbUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class NormalPlayerInput : MonoBehaviour
    {
        private Player player;
        private PlayerInput playerInput;

        // [for inputHelper]
        public EasyEvent<float> OnMoveStart = new(), OnMoveCanceled = new();
        public EasyEvent<float> OnJumpStart = new(), OnJumpCanceled = new();
        public EasyEvent<float> OnAttackStart = new(), OnAttackCanceled = new();

        readonly PlayerPlayerInputHelper inputHelper = new();

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
        private class InternalHelper
        {
            private Dictionary<string, System.Action<InputAction.CallbackContext>> actionHandlers = new Dictionary<string, System.Action<InputAction.CallbackContext>>();

            public void InternalProcessPhase<T>(InputAction.CallbackContext ctx, System.Action<T> onStart, System.Action<T> onCanceled)
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

            public bool TryTriggerAction(InputAction.CallbackContext ctx, string actionName, System.Action<InputAction.CallbackContext> handler)
            {
                var res = ctx.action.name == actionName;
                if(res) handler(ctx);
                return res;
            }
            public void RegisterActionHandler(string actionName, System.Action<InputAction.CallbackContext> handler) => actionHandlers[actionName] = handler;
            public void UnRegisterActionHandler(string actionName) => actionHandlers.Remove(actionName);
            public bool OnPlayerInputTriggered(InputAction.CallbackContext ctx)
            {
                var res = actionHandlers.TryGetValue(ctx.action.name, out var handler);
                if(res) handler(ctx);
                return res;
            }
        }

        private InternalHelper internalHelper = new InternalHelper();

        public System.Action<float> onMoveStart, onMoveCanceled;
        public System.Action<float> onJumpStart, onJumpCanceled;
        public System.Action<float> onAttackStart, onAttackCanceled;

        public PlayerPlayerInputHelper()
        {
            internalHelper.RegisterActionHandler("HorizontalMove", ProcessMovePhase);
            internalHelper.RegisterActionHandler("Jump", ProcessJumpPhase);
            internalHelper.RegisterActionHandler("Fire", ProcessAttackPhase);
        }

        public void OnPlayerInputTriggered(InputAction.CallbackContext ctx) => internalHelper.OnPlayerInputTriggered(ctx);

        private void ProcessJumpPhase(InputAction.CallbackContext ctx) => internalHelper.InternalProcessPhase(ctx, onJumpStart, onJumpCanceled);
        private void ProcessMovePhase(InputAction.CallbackContext ctx) => internalHelper.InternalProcessPhase(ctx, onMoveStart, onMoveCanceled);
        private void ProcessAttackPhase(InputAction.CallbackContext ctx) => internalHelper.InternalProcessPhase(ctx, onAttackStart, onAttackCanceled);
    }
}
