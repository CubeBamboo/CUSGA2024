using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay.Move;
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

        private SmoothMoveCtrl _moveController;
        private PlayerModel _playerModel;
        private bool attackingLock;

        // [behave state?]
        private float moveParam;

        private NormalPlayerInput mPlayerInput;

        public EasyEvent<float> OnMoveStart { get; } = new();
        public EasyEvent<bool> OnWeaponAttack { get; } = new();

        private UnityEntryPointScheduler _scheduler;
        private RuntimeContext _containerContext;
        private PlayerJumpProxy _playerJumpProxy;
        private PlayerAttackProxy _playerAttackProxy;

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

        private void Awake()
        {
            _scheduler = UnityEntryPointScheduler.Create(gameObject);
            ConfigureDependency();
            ConfigureInputEvent();

            ConfigProxy();
        }

        private void ConfigProxy()
        {
            // jump
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

            // attack
            var attackDependencies = new ServiceLocator();
            attackDependencies.AddParent(_containerContext);

            attackDependencies.RegisterInstance(mPlayerInput);
            attackDependencies.RegisterInstance(transform);
            attackDependencies.RegisterInstance(_attackSettings);
            attackDependencies.RegisterInstance(this);
            _playerAttackProxy = new PlayerAttackProxy(_scheduler, attackDependencies);
            _playerAttackProxy.Forget();
        }

        private void Start()
        {
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
        }

        private void ConfigureDependency()
        {
            var monoContainer = GetComponent<MonoContainer>();
            monoContainer.MakeSureInit();
            _containerContext = monoContainer.Context;
            _containerContext
                .Resolve(out _moveController)
                .Resolve(out _playerModel);

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
