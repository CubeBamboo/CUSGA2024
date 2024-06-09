using CbUtils;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using Shuile.Gameplay.Weapon;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public class NormalPlayerCtrl : MonoBehaviour, IEntity
    {
        private enum MainState
        {
            Normal,
            JumpUpAndInputHoldOn,
            JumpUpAndInputHoldOff,
            Drop
        }
        private enum MoveState
        {
            Idle,
            Move,
        }
        private enum JumpState
        {
            Idle,
            Jump,
        }

        // [behave state?]
        private float moveParam;

        private readonly FSM<MainState> mainFsm = new();
        private readonly FSM<MoveState> moveFsm = new();
        private readonly FSM<JumpState> jumpFsm = new();

        private NormalPlayerInput mPlayerInput;
        private SmoothMoveCtrl _moveController;

        // [normal move]
        [SerializeField] private float acc = 1.7f;
        [SerializeField] private float deAcc = 0.7f;
        [SerializeField] private float xMaxSpeed = 6.5f;

        // [hold jump]
        [SerializeField] private float jumpStartVel = 14f;
        [SerializeField] private float jumpMaxDuration = 0.27f;
        [SerializeField] private float holdJumpVelAdd = 0.65f;
        [SerializeField] private float normalGravity = 3f;
        [SerializeField] private float dropGravity = 6f;
        [SerializeField] private float holdOffYDamping = 0.4f;

        // [attack]
        [SerializeField] private float attackRadius = 3.2f;
        [SerializeField] private int attackPoint = 20;
        [SerializeField] private Transform handTransform;
        private bool attackingLock;

        private MusicRhythmManager _musicRhythmManager;
        private PlayerChartManager _playerChartManager;
        private PlayerModel _playerModel;

        private AttackCommand _attackCommand;

        public EasyEvent OnTouchGround { get; } = new();
        public EasyEvent<float> OnMoveStart { get; } = new();
        public EasyEvent<WeaponHitData> OnWeaponHit { get; } = new();
        public EasyEvent<bool> OnWeaponAttack { get; } = new();

        public EasyEvent OnJumpStart = new();

        // it has bug (((((
        public bool AttackingLock
        {
            get => attackingLock;
            set
            {
                Debug.Log("lock changed: " + value);

                if (attackingLock == value) return;
                attackingLock = value;
                //if (!value)
                //    StopAttack();

                _moveController.XMaxSpeed = value ? xMaxSpeed * 0.3f : xMaxSpeed;
                if (value && Mathf.Abs(_moveController.Velocity.x) > _moveController.XMaxSpeed)
                    _moveController.Velocity = _moveController.Velocity.With(x: Mathf.Sign(_moveController.Velocity.x) * _moveController.XMaxSpeed);
            }
        }

        private TryHitNoteCommand hitNoteCommand;

        public bool CheckRhythm
        {
            get
            {
                hitNoteCommand.inputTime = _musicRhythmManager.CurrentTime;
                this.ExecuteCommand<TryHitNoteCommand>(hitNoteCommand);
                _playerModel.currentHitOffset = hitNoteCommand.result.hitOffset;
                return hitNoteCommand.result.isHitOn;
            }
        }
        private readonly SimpleDurationTimer holdJumpTimer = new();

        private void Awake()
        {
            ConfigureDependency();
            ConfigureInputEvent();

            _attackCommand = new()
            {
                position = transform.position,
                attackRadius = attackRadius,
                attackPoint = attackPoint
            };
            hitNoteCommand = new()
            {
                musicRhythmManager = _musicRhythmManager,
                playerChartManager = _playerChartManager,
                inputTime = _musicRhythmManager.CurrentTime
            };

            holdJumpTimer.MaxDuration = jumpMaxDuration;
        }

        private void OnDestroy()
        {
            mPlayerInput.ClearAll();
        }

        private void Start()
        {
            InitializeMainFSM();
            InitializeOtherFSM();
            RefreshParameter();
        }

        private void FixedUpdate()
        {
            mainFsm.FixedUpdate();
            moveFsm.FixedUpdate();
            jumpFsm.FixedUpdate();

#if UNITY_EDITOR
            RefreshParameter();
#endif
        }

        /// <summary> update velocity </summary>
        public void NormalMove(float xInput)
        {
            _moveController.XMove(xInput);
            OnMoveStart?.Invoke(xInput);
            _playerModel.faceDir = xInput;
        }

        public void JumpPress()
        {
            if (mainFsm.CurrentStateId != MainState.Normal) return;

            _moveController.Velocity = _moveController.Velocity.With(y: jumpStartVel);
            holdJumpTimer.StartTime = Time.time;
            mainFsm.SwitchState(MainState.JumpUpAndInputHoldOn);
            OnJumpStart.Invoke();
        }

        public void HoldJump()
        {
            if (mainFsm.CurrentStateId != MainState.JumpUpAndInputHoldOn) return;

            _moveController.Velocity += new Vector2(0, holdJumpVelAdd);
            var hitWall = Mathf.Abs(_moveController.Velocity.y) < 1e-4; // ...

            if (holdJumpTimer.HasReachTime(Time.time) || hitWall)
            {
                //Debug.Log("Stop Jump Hold On Acceleration"); // end timer
                mainFsm.SwitchState(MainState.JumpUpAndInputHoldOff);
            }
        }
        public void JumpRelese()
        {
            if (mainFsm.CurrentStateId != MainState.JumpUpAndInputHoldOn) return;
            mainFsm.SwitchState(MainState.JumpUpAndInputHoldOff);
        }

        public void Attack()
        {
            if (LevelRoot.Instance.needHitWithRhythm && !CheckRhythm) return;

            _attackCommand.position = transform.position;
            this.ExecuteCommand<AttackCommand>(_attackCommand);

            OnWeaponAttack.Invoke(true);
        }

        private void CheckGround()
        {
            if (_moveController.IsOnGround)
            {
                mainFsm.SwitchState(MainState.Normal);
                OnTouchGround.Invoke();
            }
        }

        private void RefreshParameter()
        {
            _moveController.IsFrozen = false;
            _moveController.Acceleration = acc;
            _moveController.Deceleration = deAcc;
            _moveController.XMaxSpeed = xMaxSpeed;

            holdJumpTimer.MaxDuration = jumpMaxDuration;
        }

        private void InitializeMainFSM()
        {
            mainFsm
                .NewEventState(MainState.Normal)
                .OnEnter(() => _moveController.Gravity = normalGravity);
            mainFsm
                .NewEventState(MainState.JumpUpAndInputHoldOff)
                .OnEnter(() =>
                {
                    _moveController.Gravity = dropGravity;
                    jumpFsm.SwitchState(JumpState.Idle);
                })
                .OnFixedUpdate(() =>
                {
                    CheckGround();
                    _moveController.Velocity -= new Vector2(0, holdOffYDamping);

                    if (_moveController.Velocity.y < 0)
                        mainFsm.SwitchState(MainState.Drop);
                });
            mainFsm
                .NewEventState(MainState.Drop)
                .OnEnter(() => _moveController.Gravity = dropGravity)
                .OnFixedUpdate(() => CheckGround());
            mainFsm.StartState(MainState.Normal);
        }

        private void InitializeOtherFSM()
        {
            moveFsm
                .NewEventState(MoveState.Idle);
            moveFsm
                .NewEventState(MoveState.Move)
                .OnFixedUpdate(() => NormalMove(moveParam));
            moveFsm.StartState(MoveState.Idle);

            jumpFsm
                .NewEventState(JumpState.Idle);
            jumpFsm
                .NewEventState(JumpState.Jump)
                .OnEnter(JumpPress)
                .OnFixedUpdate(HoldJump)
                .OnExit(JumpRelese);
            jumpFsm.StartState(JumpState.Idle);
        }

        private void ConfigureInputEvent()
        {
            mPlayerInput.OnMoveStart.Register((v) =>
            {
                moveFsm.SwitchState(MoveState.Move);
                moveParam = v;
            });
            mPlayerInput.OnMoveCanceled.Register(_ => moveFsm.SwitchState(MoveState.Idle));

            mPlayerInput.OnJumpStart.Register(_ => jumpFsm.SwitchState(JumpState.Jump));
            mPlayerInput.OnJumpCanceled.Register(_ => jumpFsm.SwitchState(JumpState.Idle));

            mPlayerInput.OnAttackStart.Register(_ => Attack());
        }

        private void ConfigureDependency()
        {
            var scope = LevelScope.Interface;
            
            _playerModel = this.GetModel<PlayerModel>();
            mPlayerInput = GetComponent<NormalPlayerInput>();
            _moveController = GetComponent<SmoothMoveCtrl>();
            _musicRhythmManager = MusicRhythmManager.Instance;
            _playerChartManager = scope.Get<PlayerChartManager>();
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
