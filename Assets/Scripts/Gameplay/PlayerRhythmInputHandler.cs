using UnityEngine;
using CbUtils;
using Shuile.Rhythm;
using Shuile.Framework;
using static Shuile.Gameplay.PlayerRhythmInputHandler;

using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    //处理玩家输入，联系playerInput和playercontroller
    public class PlayerRhythmInputHandler : MonoBehaviour, IComponent<Player>
    {
        private IPlayerCtrl player;

        // dependency
        private PlayerModel playerModel;

        public enum State
        {
            OnGround,
            Jump
        }
        private FSM<State> mFSM = new();

        public Keyboard keyboard;

        public IPlayerCtrl Player => player;
        public bool CheckRhythm => MusicRhythmManager.Instance.CheckBeatRhythm(MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset);

        private Player mTarget;
        public Player Target { set => mTarget = value; }

        private void Start()
        {
            player = this.GetComponent<IPlayerCtrl>();
            playerModel = GameplayService.Interface.Get<PlayerModel>();

            keyboard = Keyboard.current; //TODO: refactor after demo complete

            // init fsm
            mFSM.AddState(State.OnGround, new PlayerOnGroundState(mFSM, this));
            mFSM.AddState(State.Jump, new PlayerJumpState(mFSM, this));
            mFSM.StartState(State.OnGround);
        }

        private void FixedUpdate()
        {
            mFSM.FixedUpdate();
        }

        private void Update()
        {
            mFSM.Update();
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            //var guiStyle = GUI.skin.label;
            //guiStyle.fontSize = 40;
            //guiStyle.alignment = TextAnchor.UpperLeft;
            //guiStyle.contentOffset = new Vector2(0, 400);
            //GUILayout.Label($"hitOffset:{playerModel.currentHitOffset}");
            //GUILayout.Label($"CurrentTime:{MusicRhythmManager.Instance.CurrentTime}");
            //GUILayout.Label($"RealGameTime:{Time.fixedTime}");

            //if (!MusicRhythmManager.Instance.IsPlaying)
            //    return;

            //GUILayout.Label($"RhythmCheckTime:{MusicRhythmManager.Instance.CurrentTime}");
        }
#endif

        /*private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (context.action.name != "Attack" && context.action.name != "HorizontalMove")
                return;
            if (context.phase != InputActionPhase.Started)
                return;

            switch (context.action.name)
            {
                case "Attack":
                    if (CheckRhythm)
                        player.Attack();
                    break;
                case "HorizontalMove":
                    player.GridMove(context.ReadValue<float>());
                    break;
            }

        }*/
    }

    internal class PlayerOnGroundState : BaseState<State, PlayerRhythmInputHandler>
    {
        public PlayerOnGroundState(FSM<State> fsm, PlayerRhythmInputHandler target)
            : base(fsm, target)
        {
        }

        public override void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            // jump
            bool jumpInput = keyboard.spaceKey.wasPressedThisFrame;
            if (jumpInput)
            {
                target.Player.SingleJump();
                fsm.SwitchState(State.Jump);
            }

            // move
            bool moveInput = keyboard.aKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame;
            if (moveInput)
            {
                int dir = keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame ? -1 : 1;
                target.Player.NormalMove(dir);
            }

            // attack
            bool attackInput = keyboard.jKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame;
            if(attackInput && target.CheckRhythm)
            {
                target.Player.Attack();
            }
        }
    }

    internal class PlayerJumpState : BaseState<State, PlayerRhythmInputHandler>
    {
        public PlayerJumpState(FSM<State> fsm, PlayerRhythmInputHandler target)
            : base(fsm, target)
        {
        }

        //public override void Update()
        //{
        //    // dash
        //    //if (target.keyboard.aKey.wasPressedThisFrame || target.keyboard.dKey.wasPressedThisFrame)
        //    //{
        //    //    int dir = target.keyboard.aKey.wasPressedThisFrame ? -1 : 1;
        //    //    target.Player.Dash(dir);
        //    //}
        //}

        public override void FixedUpdate()
        {
            // 检测落地
            Vector2 origin = (Vector2)target.transform.position + new Vector2(0f, -1.0f); // close to player's foot
            var hit = UnityAPIExt.RayCast2DWithDebugLine(origin, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            if (hit) fsm.SwitchState(State.OnGround); //TODO: check immediately when not jump to air will also get "true" result (it's a bug).
        }
    }
}
