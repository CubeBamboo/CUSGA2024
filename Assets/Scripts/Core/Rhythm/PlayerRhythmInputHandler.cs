using UnityEngine;
using UnityEngine.InputSystem;
using CbUtils;
using static Shuile.Gameplay.PlayerRhythmInputHandler;
using Shuile.Rhythm;

namespace Shuile.Gameplay
{
    //处理玩家输入，联系playerInput和playercontroller
    public class PlayerRhythmInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private PlayerController player;

        // dependency
        private PlayerModel playerModel;

        public enum PlayerState
        {
            OnGround,
            Jump
        }
        private FSM<PlayerState> mFSM = new();

        public Keyboard keyboard;

        public PlayerController Player => player;

        private void Start()
        {
            playerModel = GameplayService.Interface.Get<PlayerModel>();

            keyboard = Keyboard.current; //TODO: refactor after demo complete
            playerInput.onActionTriggered += OnActionTriggered;

            // init fsm
            mFSM.AddState(PlayerState.OnGround, new PlayerOnGroundState(mFSM, this));
            mFSM.AddState(PlayerState.Jump, new PlayerJumpState(mFSM, this));
            mFSM.StartState(PlayerState.OnGround);
        }

        private void OnDestroy()
        {
            playerInput.onActionTriggered -= OnActionTriggered;
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

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (context.action.name != "Attack" && context.action.name != "HorizontalMove")
                return;
            if (context.phase != InputActionPhase.Started)
                return;
            if (!MusicRhythmManager.Instance.CheckBeatRhythm(MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset))
                return;

            switch (context.action.name)
            {
                case "Attack":
                    player.Attack();
                    break;
                case "HorizontalMove":
                    player.Move(context.ReadValue<float>());
                    break;
            }

        }
    }

    internal class PlayerOnGroundState : BaseState<PlayerState, PlayerRhythmInputHandler>
    {
        public PlayerOnGroundState(FSM<PlayerState> fsm, PlayerRhythmInputHandler target)
            : base(fsm, target)
        {
        }

        public override void Update()
        {
            // 检测跳跃
            if (target.keyboard.wKey.wasPressedThisFrame)
            {
                target.Player.Jump();
                fsm.SwitchState(PlayerState.Jump);
            }
        }
    }

    internal class PlayerJumpState : BaseState<PlayerState, PlayerRhythmInputHandler>
    {
        public PlayerJumpState(FSM<PlayerState> fsm, PlayerRhythmInputHandler target)
            : base(fsm, target)
        {
        }

        public override void Update()
        {
            // 检测冲刺
            if (target.keyboard.aKey.wasPressedThisFrame || target.keyboard.dKey.wasPressedThisFrame)
            {
                int dir = target.keyboard.aKey.wasPressedThisFrame ? -1 : 1;
                target.Player.Dash(dir);
            }
        }

        public override void FixedUpdate()
        {
            // 检测落地
            Vector2 origin = (Vector2)target.transform.position + new Vector2(0f, -1.0f); // close to player's foot
            var hit = UnityAPIExt.RayCast2DWithDebugLine(origin, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            if (hit) fsm.SwitchState(PlayerState.OnGround); //TODO: check immediately when not jump to air will also get "true" result (it's a bug).
        }
    }
}
