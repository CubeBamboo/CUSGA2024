using CbUtils;
using CbUtils.Editor;
using Shuile.Framework;
using Shuile.Gameplay.Weapon;
using Shuile.Rhythm.Runtime;
using Shuile.Root;

using System.Linq;
using UnityEngine;

using static UnityEngine.UI.CanvasScaler;

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
        public enum MainState
        {
            Normal,
            JumpUpAndInputHoldOn,
            JumpUpAndInputHoldOff,
            Drop
        }

        // [behave state?]
        private bool isMoving, isJumping;
        private float moveParam;

        private FSM<MainState> mainFsm = new();

        private Player player;
        private NormalPlayerInput mPlayerInput;
        private SmoothMoveCtrl _moveController;

        // [normal move]
        [SerializeField] private float acc = 1.7f;
        [SerializeField] private float deAcc = 0.7f;
        [SerializeField] private float xMaxSpeed = 6.5f;

        // [jump]

        // [hold jump]
        [SerializeField] private float jumpStartVel = 14f;
        [SerializeField] private float jumpMaxDuration = 0.27f;
        [SerializeField] private float holdJumpVelAdd = 0.65f;
        [SerializeField] private float normalGravity = 3f;
        [SerializeField] private float dropGravity = 6f;
        [SerializeField] private float holdOffYDamping = 0.4f;

        // [attack]
        [SerializeField] private Transform handTransform;
        private IWeapon currentWeapon;
        private bool attackingLock;

        private PlayerModel playerModel;

        private PlayerMoveCommand moveCommand = new();
        public EasyEvent OnTouchGround { get; } = new();
        public EasyEvent<float> OnMoveStart { get; } = new();
        public EasyEvent<WeaponHitData> OnWeaponHit { get; } = new();
        public EasyEvent<bool> OnWeaponAttack { get; } = new();

        public EasyEvent OnJumpStart = new();

        public IWeapon CurrentWeapon
        {
            get => currentWeapon;
            set
            {
                if (AttackingLock)
                {
                    Debug.LogWarning("Not support to change weapon while attack locking");
                    return;
                }
                if (currentWeapon != null)
                {
                    currentWeapon.OnHit.UnRegister(OnWeaponHit.Invoke);
                    currentWeapon.BindToTransform(null);
                }
                currentWeapon = value;
                if (currentWeapon != null)
                {
                    currentWeapon.OnHit.Register(OnWeaponHit.Invoke);
                    currentWeapon.BindToTransform(handTransform);
                }
            }
        }
        public bool IsWeaponExist => currentWeapon != null && currentWeapon.GetType() != typeof(NoWeapon);
        public bool AttackingLock
        {
            get => attackingLock;
            set
            {
                if (attackingLock == value) return;
                attackingLock = value;
                if (!value)
                    StopAttack();

                _moveController.XMaxSpeed = value ? xMaxSpeed * 0.3f : xMaxSpeed;
                if (value && Mathf.Abs(_moveController.Velocity.x) > _moveController.XMaxSpeed)
                    _moveController.Velocity = _moveController.Velocity.With(x: Mathf.Sign(_moveController.Velocity.x) * _moveController.XMaxSpeed);
            }
        }

        public bool CheckRhythm =>
            PlayerChartManager.Instance.TryHit(
                MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset);

        private SimpleDurationTimer holdJumpTimer = new();

        //private StateCheckDebugProperty _checkDebugProperty;
        private void Awake()
        {
            player = GetComponent<Player>();
            mPlayerInput = GetComponent<NormalPlayerInput>();
            _moveController = GameplayService.Interface.Get<PlayerModel>().moveCtrl;
            holdJumpTimer.MaxDuration = jumpMaxDuration;
            CurrentWeapon = handTransform.GetComponentInChildren<IWeapon>() ?? new NoWeapon();

            ConfigureDependency();
            ConfigureInputEvent();
        }

        private void OnDestroy()
        {
            ClearInputEvent();
        }

        private void Start()
        {
            InitMainFSM();
            _moveController.IsFrozen = false;
            _moveController.Acceleration = acc;
            _moveController.Deceleration = deAcc;
            _moveController.XMaxSpeed = xMaxSpeed;
        }

        private void FixedUpdate()
        {
            mainFsm.FixedUpdate();
            BehaveUpdate();
            RefreshParameter();
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
            OnMoveStart?.Invoke(xInput);
            playerModel.faceDir = xInput;
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
                //Debug.Log("Stop Jump Hold On Acceleration");
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

            CurrentWeapon.AttackCommand
                .Bind(new(transform.position, new Vector2(Mathf.Sign(playerModel.faceDir), 0f)))
                .Execute();
            OnWeaponAttack.Invoke(true);
        }

        public void StopAttack()
        {
            if (attackingLock)
                return;

            CurrentWeapon.AttackFinishCommand
                .Bind(new(transform.position, new Vector2(Mathf.Sign(playerModel.faceDir), 0f)))
                .Execute();
            OnWeaponAttack.Invoke(false);
        }

        // Shortcut to invoke by animator
        public void ReleaseAttackingLock()
            => AttackingLock = false;

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

        private void BehaveUpdate()
        {
            if (isMoving)
            {
                NormalMove(moveParam);
            }

            if (isJumping)
            {
                HoldJump();
            }
        }

        private void ConfigureDependency()
        {
            playerModel = GameplayService.Interface.Get<PlayerModel>();
        }

        private void ConfigureInputEvent()
        {
            mPlayerInput.OnMoveStart.Register((v) =>
            {
                isMoving = true;
                moveParam = v;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            mPlayerInput.OnMoveCanceled.Register(v => isMoving = false)
              .UnRegisterWhenGameObjectDestroyed(gameObject);

            mPlayerInput.OnJumpStart.Register(v =>
            {
                JumpPress();
                isJumping = true;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            mPlayerInput.OnJumpCanceled.Register(v =>
            {
                isJumping = false;
                JumpRelese();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            mPlayerInput.OnAttackStart.Register(v => Attack())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            mPlayerInput.OnAttackCanceled.Register(v => StopAttack())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

        }

        private void ClearInputEvent()
        {
        }

        private void InitMainFSM()
        {
            mainFsm.NewEventState(MainState.Normal)
                .OnEnter(() =>
                {
                    _moveController.Gravity = normalGravity;

                    //Debug.Log("Enter Normal");
                });
            mainFsm.NewEventState(MainState.JumpUpAndInputHoldOff)
                .OnEnter(() =>
                {
                    _moveController.Gravity = dropGravity;
                    isJumping = false;
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
            mainFsm.NewEventState(MainState.Drop)
                .OnEnter(() =>
                {
                    _moveController.Gravity = dropGravity;
                })
                .OnFixedUpdate(() =>
                {
                    CheckGround();
                });
            mainFsm.StartState(MainState.Normal);

            //mFsm.OnStateChanged += (o, n) =>
            //{
            //    Debug.Log($"{o} -> {n}");
            //};
        }
    }

    //public struct PlayerJumpCommandData
    //{
    //    public SmoothMoveCtrl moveController;
    //    public float jumpVel;
    //}
    //public class PlayerJumpCommand : BaseCommand<PlayerJumpCommandData>
    //{
    //    public override void OnExecute()
    //    {
    //        state.moveController.JumpVelocity = state.jumpVel;
    //        state.moveController.SimpleJump(1f);
    //    }
    //}

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
