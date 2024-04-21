using CbUtils;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;

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
        public float XInput { get; private set; }

        public event System.Action OnJump, OnAttack;
        public event System.Action<float> OnMove;

        private void Awake()
        {
            player = GetComponent<Player>();
        }
        private void Start()
        {
            player.OnDie.Register(() => this.enabled = false)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            // jump
            bool jumpInput = keyboard.spaceKey.wasPressedThisFrame;
            if (jumpInput)
            {
                OnJump?.Invoke();
            }

            // move
            bool moveInput = keyboard.aKey.isPressed || keyboard.dKey.isPressed || keyboard.leftArrowKey.isPressed || keyboard.rightArrowKey.isPressed;
            if (moveInput)
            {
                int dir = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1 : 1;
                XInput = dir;
                OnMove?.Invoke(dir);
            }

            // attack
            bool attackInput = keyboard.jKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame;
            if (attackInput)
            {
                OnAttack?.Invoke();
            }
        }
    }
}
