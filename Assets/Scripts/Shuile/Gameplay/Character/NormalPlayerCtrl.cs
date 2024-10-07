using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using Shuile.Gameplay.Weapon;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl : MonoBehaviour
    {
        [SerializeField] private MoveSettings _moveSettings = new();
        [SerializeField] private JumpSettings _jumpSettings = new();
        [SerializeField] private AttackSettings _attackSettings = new();

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

                _moveController.XMaxSpeed = value ? _moveSettings.xMaxSpeed * 0.3f : _moveSettings.xMaxSpeed;
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
                jumpStartVel = _jumpSettings.jumpStartVel,
                holdJumpVelAdd = _jumpSettings.holdJumpVelAdd,
                jumpMaxDuration = _jumpSettings.jumpMaxDuration,
                normalGravity = _jumpSettings.normalGravity,
                dropGravity = _jumpSettings.dropGravity,
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
                position = transform.position, attackRadius = _attackSettings.attackRadius, attackPoint = _attackSettings.attackPoint
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
            _moveController.Acceleration = _moveSettings.acc;
            _moveController.Deceleration = _moveSettings.deAcc;
            _moveController.XMaxSpeed = _moveSettings.xMaxSpeed;
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

        [Serializable]
        public class MoveSettings
        {
            public float acc = 1.7f;
            public float deAcc = 0.7f;
            public float xMaxSpeed = 6.5f;
        }

        [Serializable]
        public class JumpSettings
        {
            public float jumpStartVel = 14f;
            public float jumpMaxDuration = 0.27f;
            public float holdJumpVelAdd = 0.65f;
            public float normalGravity = 3f;
            public float dropGravity = 6f;
        }

        [Serializable]
        public class AttackSettings
        {
            public float attackRadius = 3.2f;
            public int attackPoint = 20;
        }
    }
}
