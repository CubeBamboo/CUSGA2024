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
        [SerializeField] private AnimationCurve _accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _deAccelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        // private SmoothMoveCtrl _moveController; // only used for refresh parameter in unity editor

        private NormalPlayerInput mPlayerInput;

        public EasyEvent<bool> OnWeaponAttack { get; } = new();

        private UnityEntryPointScheduler _scheduler;
        private RuntimeContext _containerContext;
        private PlayerJumpProxy _playerJumpProxy;
        private PlayerAttackProxy _playerAttackProxy;
        private PlayerMoveProxy _playerMoveProxy;

        private void Awake()
        {
            _scheduler = UnityEntryPointScheduler.Create(gameObject);
            ConfigureDependency();
            ConfigProxy();
        }

        private void Start()
        {
            RefreshParameter();
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            RefreshParameter();
#endif
        }

        private void OnDestroy()
        {
            mPlayerInput.ClearAll();
        }

        private void RefreshParameter()
        {
            // _moveController.IsFrozen = false;
            // _moveController.Acceleration = _moveSettings.acc;
            // _moveController.Deceleration = _moveSettings.deAcc;
            // _moveController.XMaxSpeed = _moveSettings.xMaxSpeed;

            _playerJumpProxy.RefreshSettings(BuildPlayerJump(_jumpSettings));
        }

        private void ConfigureDependency()
        {
            var monoContainer = GetComponent<MonoContainer>();
            monoContainer.MakeSureInit();
            _containerContext = monoContainer.Context;
            // _containerContext
            //     .Resolve(out _moveController);

            mPlayerInput = GetComponent<NormalPlayerInput>();
        }

        private void ConfigProxy()
        {
            // move
            var moveDependencies = new ServiceLocator();
            moveDependencies.AddParent(_containerContext);

            moveDependencies.RegisterInstance(gameObject);
            moveDependencies.RegisterInstance(mPlayerInput);
            moveDependencies.RegisterInstance(_moveSettings);
            _playerMoveProxy = new PlayerMoveProxy(_scheduler, moveDependencies, _accelerationCurve, _deAccelerationCurve);
            _playerMoveProxy.Forget();

            // jump
            var jumpDependencies = new ServiceLocator();
            jumpDependencies.AddParent(_containerContext);

            jumpDependencies.RegisterInstance(BuildPlayerJump(_jumpSettings));
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

        private PlayerJumpProxy.Settings BuildPlayerJump(JumpSettings jumpSettings) => new PlayerJumpProxy.Settings
        {
            jumpStartVel = jumpSettings.jumpStartVel,
            holdJumpVelAdd = jumpSettings.holdJumpVelAdd,
            jumpMaxDuration = jumpSettings.jumpMaxDuration,
            normalGravity = jumpSettings.normalGravity,
            dropGravity = jumpSettings.dropGravity,
            onInputJumpStart = mPlayerInput.OnJumpStart,
            onInputJumpCanceled = mPlayerInput.OnJumpCanceled,
        };

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
