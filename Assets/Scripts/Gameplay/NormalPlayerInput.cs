using CbUtils;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;

using UnityEngine;
using UnityEngine.InputSystem;

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
            mTarget.OnDie.Register(() => this.enabled = false)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
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

            // NORMAL
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
                    if (attackInput && (!LevelRoot.Instance.needHitWithRhythm || CheckRhythm))
                    {
                        playerCtrl.Attack();
                    }
                });

            // JUMP
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
                    if (attackInput && (!LevelRoot.Instance.needHitWithRhythm || CheckRhythm))
                    {
                        playerCtrl.Attack();
                    }
                })
                .OnFixedUpdate(() =>
                {
                    var groundCheck = playerCtrl.Rb.velocity.y < 0 &&
                        UnityAPIExt.RayCast2DWithDebugLine(transform.position + new Vector3(0, -1.3f, 0), Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
                    //var groundCheck = true; // ...
                    if (groundCheck)
                    {
                        mFsm.SwitchState(State.Normal);
                        playerCtrl.TouchGround();
                    }
                });

            mFsm.StartState(State.Normal);
        }
    }
}
