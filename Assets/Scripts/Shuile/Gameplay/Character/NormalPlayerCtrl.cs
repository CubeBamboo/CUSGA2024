using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using Shuile.Gameplay.Weapon;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public class NormalPlayerCtrl : MonoBehaviour
    {
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
        private readonly SimpleDurationTimer _holdJumpTimer = new();
        private readonly FSM<JumpState> jumpFsm = new();

        private readonly FSM<MainState> mainFsm = new();
        private readonly FSM<MoveState> moveFsm = new();

        private AttackCommand _attackCommand;
        private TryHitNoteCommand _hitNoteCommand;
        private SmoothMoveCtrl _moveController;

        private MusicRhythmManager _musicRhythmManager;
        private PlayerChartManager _playerChartManager;
        private PlayerModel _playerModel;
        private bool attackingLock;

        // [behave state?]
        private float moveParam;

        private NormalPlayerInput mPlayerInput;

        public EasyEvent OnJumpStart = new();

        public EasyEvent OnTouchGround { get; } = new();
        public EasyEvent<float> OnMoveStart { get; } = new();
        public EasyEvent<WeaponHitData> OnWeaponHit { get; } = new();
        public EasyEvent<bool> OnWeaponAttack { get; } = new();

        // it has bug (((((
        public bool AttackingLock
        {
            get => attackingLock;
            set
            {
                Debug.Log("lock changed: " + value);

                if (attackingLock == value)
                {
                    return;
                }

                attackingLock = value;
                //if (!value)
                //    StopAttack();

                _moveController.XMaxSpeed = value ? xMaxSpeed * 0.3f : xMaxSpeed;
                if (value && Mathf.Abs(_moveController.Velocity.x) > _moveController.XMaxSpeed)
                {
                    _moveController.Velocity =
                        _moveController.Velocity.With(
                            Mathf.Sign(_moveController.Velocity.x) * _moveController.XMaxSpeed);
                }
            }
        }

        public bool CheckRhythm
        {
            get
            {
                _hitNoteCommand.inputTime = _musicRhythmManager.CurrentTime;
                _hitNoteCommand.Execute();
                _playerModel.currentHitOffset = _hitNoteCommand.result.hitOffset;
                return _hitNoteCommand.result.isHitOn;
            }
        }

        private void Awake()
        {
            ConfigureDependency();
            ConfigureInputEvent();

            _holdJumpTimer.MaxDuration = jumpMaxDuration;
        }

        private void Start()
        {
            _attackCommand = new AttackCommand
            {
                position = transform.position, attackRadius = attackRadius, attackPoint = attackPoint
            };
            _hitNoteCommand = new TryHitNoteCommand
            {
                musicRhythmManager = _musicRhythmManager,
                playerChartManager = _playerChartManager,
                inputTime = _musicRhythmManager.CurrentTime
            };

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

        private void OnDestroy()
        {
            mPlayerInput.ClearAll();
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
            if (mainFsm.CurrentStateId != MainState.Normal)
            {
                return;
            }

            _moveController.Velocity = _moveController.Velocity.With(y: jumpStartVel);
            _holdJumpTimer.StartTime = Time.time;
            mainFsm.SwitchState(MainState.JumpUpAndInputHoldOn);
            OnJumpStart.Invoke();
        }

        public void HoldJump()
        {
            if (mainFsm.CurrentStateId != MainState.JumpUpAndInputHoldOn)
            {
                return;
            }

            _moveController.Velocity += new Vector2(0, holdJumpVelAdd);
            var hitWall = Mathf.Abs(_moveController.Velocity.y) < 1e-4; // ...

            if (_holdJumpTimer.HasReachTime(Time.time) || hitWall)
            {
                //Debug.Log("Stop Jump Hold On Acceleration"); // end timer
                mainFsm.SwitchState(MainState.JumpUpAndInputHoldOff);
            }
        }

        public void JumpRelese()
        {
            if (mainFsm.CurrentStateId != MainState.JumpUpAndInputHoldOn)
            {
                return;
            }

            mainFsm.SwitchState(MainState.JumpUpAndInputHoldOff);
        }

        public void Attack()
        {
            if (LevelRoot.Instance.needHitWithRhythm && !CheckRhythm)
            {
                return;
            }

            _attackCommand.position = transform.position;
            _attackCommand.Execute();

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

            _holdJumpTimer.MaxDuration = jumpMaxDuration;
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
                    {
                        mainFsm.SwitchState(MainState.Drop);
                    }
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
            mPlayerInput.OnMoveStart.Register(v =>
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
            _playerModel = scope.GetImplementation<PlayerModel>();
            _playerChartManager = scope.GetImplementation<PlayerChartManager>();
            _musicRhythmManager = scope.GetImplementation<MusicRhythmManager>();

            mPlayerInput = GetComponent<NormalPlayerInput>();
            _moveController = GetComponent<SmoothMoveCtrl>();
        }

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
            Move
        }

        private enum JumpState
        {
            Idle,
            Jump
        }
    }
}
