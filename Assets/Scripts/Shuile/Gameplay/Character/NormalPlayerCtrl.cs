using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using Shuile.Gameplay.Weapon;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl : MonoBehaviour
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

        private readonly FSM<MoveState> moveFsm = new();

        private AttackCommand _attackCommand;
        private TryHitNoteCommand _hitNoteCommand;
        private SmoothMoveCtrl _moveController;
        private UnityEntryPointScheduler _scheduler;

        private MusicRhythmManager _musicRhythmManager;
        private PlayerChartManager _playerChartManager;
        private PlayerModel _playerModel;
        private bool attackingLock;

        // [behave state?]
        private float moveParam;

        private NormalPlayerInput mPlayerInput;

        public EasyEvent OnTouchGround { get; } = new();
        public EasyEvent<float> OnMoveStart { get; } = new();
        public EasyEvent<WeaponHitData> OnWeaponHit { get; } = new();
        public EasyEvent<bool> OnWeaponAttack { get; } = new();

        private PlayerJumpProxy _playerJumpProxy;

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
            _scheduler = UnityEntryPointScheduler.Create(gameObject);
            ConfigureDependency();
            ConfigureInputEvent();

            ConfigJumpProxy();
        }

        private void ConfigJumpProxy()
        {
            var jumpDependencies = new ServiceLocator();
            jumpDependencies.RegisterInstance(_moveController);
            jumpDependencies.RegisterInstance(new PlayerJumpProxy.Settings
            {
                jumpStartVel = jumpStartVel,
                holdJumpVelAdd = holdJumpVelAdd,
                jumpMaxDuration = jumpMaxDuration,
                normalGravity = normalGravity,
                dropGravity = dropGravity,
                onInputJumpStart = mPlayerInput.OnJumpStart,
                onInputJumpCanceled = mPlayerInput.OnJumpCanceled,
            });
            _playerJumpProxy = new PlayerJumpProxy(_scheduler, jumpDependencies);
            _playerJumpProxy.Forget();
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

            InitializeOtherFSM();
            RefreshParameter();
        }

        private void FixedUpdate()
        {
            moveFsm.FixedUpdate();

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

        private void RefreshParameter()
        {
            _moveController.IsFrozen = false;
            _moveController.Acceleration = acc;
            _moveController.Deceleration = deAcc;
            _moveController.XMaxSpeed = xMaxSpeed;
        }

        private void InitializeOtherFSM()
        {
            moveFsm
                .NewEventState(MoveState.Idle);
            moveFsm
                .NewEventState(MoveState.Move)
                .OnFixedUpdate(() => NormalMove(moveParam));
            moveFsm.StartState(MoveState.Idle);
        }

        private void ConfigureInputEvent()
        {
            mPlayerInput.OnMoveStart.Register(v =>
            {
                moveFsm.SwitchState(MoveState.Move);
                moveParam = v;
            });
            mPlayerInput.OnMoveCanceled.Register(_ => moveFsm.SwitchState(MoveState.Idle));

            mPlayerInput.OnAttackStart.Register(_ => Attack());
        }

        private void ConfigureDependency()
        {
            var monoContainer = GetComponent<MonoContainer>();
            monoContainer.MakeSureInit();
            monoContainer.Context
                .Resolve(out _moveController)
                .Resolve(out _playerModel)
                .Resolve(out _playerChartManager)
                .Resolve(out _musicRhythmManager);

            mPlayerInput = GetComponent<NormalPlayerInput>();
        }

        private enum MoveState
        {
            Idle,
            Move
        }
    }
}
