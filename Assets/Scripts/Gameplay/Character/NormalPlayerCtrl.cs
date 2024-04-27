using CbUtils;
using CbUtils.ActionKit;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;
using Shuile.Root;
using System.Linq;
using UnityEngine;

namespace Shuile.Gameplay
{
    public interface IPlayerCtrl
    {
        void NormalMove(float xInput);
        void SingleJump();
        void Attack();
    }

    public class NormalPlayerCtrl : MonoBehaviour
    {
        public enum State
        {
            Normal,
            JumpUp,
            Drop
        }
        private FSM<State> mFsm = new();

        private Player player;
        private NormalPlayerInput mPlayerInput;
        private SmoothMoveCtrl _moveController;

        // [normal move]
        [SerializeField] private float acc = 0.4f;
        [SerializeField] private float deAcc = 0.25f;
        [SerializeField] private float xMaxSpeed = 5f;

        // [jump]
        [SerializeField] private float jumpVel = 18f;
        [SerializeField] private float normalGravity = 3f;
        [SerializeField] private float dropGravity = 4f;
        
        // [attack]
        [SerializeField] private float attackRadius = 2.8f;

        private PlayerModel playerModel;

        public EasyEvent OnTouchGround { get; } = new();
        public PlayerAttackCommand attackCommand { get; } = new();
        public PlayerJumpCommand jumpCommand { get; } = new();
        public PlayerMoveCommand moveCommand { get; } = new();

        public bool CheckRhythm =>
            MusicRhythmManager.Instance.CheckBeatRhythm(
                MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset);

        private SimpleTimer attackSpeedDownTimer = new();

        public bool AttackSpeedDown
        {
            set
            {
                _moveController.XMaxSpeed = value ? xMaxSpeed * 0.3f : xMaxSpeed;
                if (value && Mathf.Abs(_moveController.Velocity.x) > _moveController.XMaxSpeed)
                    _moveController.Velocity = _moveController.Velocity.With(x: Mathf.Sign(_moveController.Velocity.x) * _moveController.XMaxSpeed);
            }
        }

        private void Awake()
        {
            player = GetComponent<Player>();
            _moveController = GetComponent<SmoothMoveCtrl>();
            mPlayerInput = GetComponent<NormalPlayerInput>();
            attackSpeedDownTimer.Add(() => AttackSpeedDown = false);

            ConfigureDependency();
            ConfigureInputEvent();
            InitFSM();
        }

        private void OnDestroy()
        {
            ClearInputEvent();
        }

        private void Start()
        {
            _moveController.IsFrozen = false;
            _moveController.Acceleration = acc;
            _moveController.Deceleration = deAcc;
            _moveController.XMaxSpeed = xMaxSpeed;
        }

        private void FixedUpdate()
        {
            mFsm.FixedUpdate();
            attackSpeedDownTimer.Tick(Time.fixedDeltaTime);

        }

        /// <summary> update velocity </summary>
        public void NormalMove(float xInput)
        {
            moveCommand
                .Bind(new()
                {
                    xInput = xInput,
                    moveController = _moveController,
                })
                .Execute();
            playerModel.faceDir = xInput;
        }

        public void SingleJump()
        {
            if (mFsm.CurrentStateId == State.JumpUp || mFsm.CurrentStateId == State.Drop) return;

            jumpCommand
                .Bind(new()
                {
                    moveController = _moveController,
                    jumpVel = jumpVel
                })
                .Execute();
            mFsm.SwitchState(State.JumpUp);
        }

        public void Attack()
        {
            if (LevelRoot.Instance.needHitWithRhythm && !CheckRhythm) return;

            attackCommand
                .Bind(new()
                {
                    position = transform.position,
                    attackRadius = attackRadius,
                    attackPoint = player.Property.attackPoint
                })
                .Execute();

            AttackSpeedDown = true;
            attackSpeedDownTimer.RestartTick();
            attackSpeedDownTimer.Duration = 0.2f;
        }

        private void CheckGround()
        {
            // easy way to fix groundcheck is true on jumping first frame, but it maybe lead to other bug
            //var groundCheck = Rb.velocity.y < 0 &&
            //      Physics2D.Raycast(transform.position + new Vector3(0, -1.3f, 0), Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
            if (_moveController.IsOnGround)
            {
                mFsm.SwitchState(State.Normal);
                OnTouchGround.Invoke();
            }
        }

        private void ConfigureDependency()
        {
            playerModel = GameplayService.Interface.Get<PlayerModel>();
        }

        private void ConfigureInputEvent()
        {
            mPlayerInput.OnJump += SingleJump;
            mPlayerInput.OnAttack += Attack;
            mPlayerInput.OnMove += NormalMove;
        }

        private void ClearInputEvent()
        {
            mPlayerInput.OnJump -= SingleJump;
            mPlayerInput.OnAttack -= Attack;
            mPlayerInput.OnMove -= NormalMove;
        }

        private void InitFSM()
        {
            mFsm.NewEventState(State.Normal)
                .OnEnter(() =>
                {
                    _moveController.Gravity = normalGravity;
                });
            mFsm.NewEventState(State.JumpUp)
                .OnFixedUpdate(() =>
                {
                    CheckGround();

                    if (_moveController.Velocity.y < 0)
                    {
                        mFsm.SwitchState(State.Drop);
                    }
                });
            mFsm.NewEventState(State.Drop)
                .OnEnter(() =>
                {
                    _moveController.Gravity = dropGravity;
                })
                .OnFixedUpdate(() =>
                {
                    CheckGround();
                });
            mFsm.StartState(State.Normal);
        }
    }

    public struct PlayerAttackCommandData
    {
        public Vector2 position;
        public float attackRadius;
        public int attackPoint;
    }
    public class PlayerAttackCommand : BaseCommand<PlayerAttackCommandData>
    {
        public override void OnExecute()
        {
            var hits = Physics2D.OverlapCircleAll(state.position, state.attackRadius, LayerMask.GetMask("Enemy"));
            var hurts = hits.Select(hit => hit.GetComponent<IHurtable>());
            foreach (var hurt in hurts)
                hurt.OnHurt(state.attackPoint);
        }
    }

    public struct PlayerJumpCommandData
    {
        public SmoothMoveCtrl moveController;
        public float jumpVel;
    }
    public class PlayerJumpCommand : BaseCommand<PlayerJumpCommandData>
    {
        public override void OnExecute()
        {
            state.moveController.JumpVelocity = state.jumpVel;
            state.moveController.SimpleJump(1f);
        }
    }

    public struct PlayerMoveCommandData
    {
        public float xInput;
        public IMoveController moveController;
    }
    public class PlayerMoveCommand : BaseCommand<PlayerMoveCommandData>
    {
        public override void OnExecute()
        {
            state.moveController.XMove(state.xInput);
        }
    }
}
