using CbUtils;
using Shuile.Framework;
using Shuile.Rhythm;

using UnityEngine;
using UnityEngine.InputSystem;
using static Shuile.Gameplay.PlayerRhythmInputHandler;

namespace Shuile.Gameplay
{
    public class NormalPlayerInput : MonoBehaviour, IComponent<Player>
    {
        public enum State
        {
            Normal,
            Jump
        }

        private NormalPlayerCtrl playerCtrl;
        private PlayerModel playerModel;
        private FSM<State> mFsm = new();

        private Keyboard _keyboard;
        private Keyboard keyboard => _keyboard != null ? _keyboard : _keyboard = Keyboard.current;

        private Player mTarget;
        public Player Target { set => mTarget = value; }

        public bool CheckRhythm => MusicRhythmManager.Instance.CheckBeatRhythm(MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset);

        private void Awake()
        {
            InitFSM();
        }

        private void Start()
        {
            playerCtrl = GetComponent<NormalPlayerCtrl>();
            playerModel = GameplayService.Interface.Get<PlayerModel>();

        }

        private void Update()
        {
            mFsm.Update(); // get input
        }
        private void FixedUpdate()
        {
            mFsm.FixedUpdate(); // update physics check
        }

        public void InitFSM()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            mFsm.NewEventState(State.Normal)
                .OnUpdate(() =>
                {
                    // jump
                    bool jumpInput = keyboard.spaceKey.wasPressedThisFrame;
                    if (jumpInput)
                    {
                        playerCtrl.SingleJump();
                        mFsm.SwitchState(State.Jump);
                    }

                    // move
                    bool moveInput = keyboard.aKey.isPressed || keyboard.dKey.isPressed || keyboard.leftArrowKey.isPressed || keyboard.rightArrowKey.isPressed;
                    if (moveInput)
                    {
                        int dir = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1 : 1;
                        playerCtrl.NormalMove(dir);
                    }

                    // attack
                    bool attackInput = keyboard.jKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame;
                    if (attackInput && CheckRhythm)
                    {
                        playerCtrl.Attack();
                    }
                });

            mFsm.NewEventState(State.Jump)
                .OnUpdate(() =>
                {
                    // move
                    bool moveInput = keyboard.aKey.isPressed || keyboard.dKey.isPressed || keyboard.leftArrowKey.isPressed || keyboard.rightArrowKey.isPressed;
                    if (moveInput)
                    {
                        int dir = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1 : 1;
                        playerCtrl.NormalMove(dir);
                    }

                    // attack
                    bool attackInput = keyboard.jKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame;
                    if (attackInput && CheckRhythm)
                    {
                        playerCtrl.Attack();
                    }
                })
                .OnFixedUpdate(() =>
                {
                    if (UnityAPIExt.RayCast2DWithDebugLine(transform.position + new Vector3(0, -0.8f, 0), Vector2.zero, 0.1f, LayerMask.GetMask("Ground"))
                        && playerCtrl.Rb.velocity.y < 0)
                    {
                        mFsm.SwitchState(State.Normal);
                    }
                });

            mFsm.StartState(State.Normal);
        }
    }
}
