using CbUtils;
using CbUtils.Event;
using DG.Tweening;
using Shuile.Gameplay.Character;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class ZakoMachine : Enemy
    {
        [SerializeField] private float checkPlayerRange = 5f;
        [SerializeField] private float checkAttackRange = 1.2f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private int attackPoint = 80;
        private readonly FSM<DefaultEnemyState> _mFsm = new();
        private ZakoChaseBehavior _chaseBehavior;

        private float _faceDir;

        private SpriteRenderer _mRenderer;

        private ZakoPatrolBehavior _patrolBehavior;
        private Player _player;

        private void Update()
        {
            _mFsm.Update();
        }

        private void FixedUpdate()
        {
            _mFsm.FixedUpdate();
        }

        protected override void OnAwake()
        {
            var scope = LevelScope.Interface;
            _player = scope.GetImplementation<Player>();

            _patrolBehavior = new ZakoPatrolBehavior(gameObject, moveController, 5f);
            _chaseBehavior = new ZakoChaseBehavior();
            RegisterState(_mFsm);

            _mRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void OnSelfDie()
        {
            _mFsm.SwitchState(DefaultEnemyState.Dead);
            moveController.IsFrozen = true;

            transform.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => Destroy(gameObject));
        }

        private void Attack(Player target)
        {
            var targetPos = target.transform.position;
            if ((targetPos - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                moveController.XMove(10f);
                target.OnHurt(attackPoint);
            }
        }

        protected void RegisterState(FSM<DefaultEnemyState> mFsm)
        {
            mFsm.NewEventState(DefaultEnemyState.Patrol)
                .OnFixedUpdate(() =>
                {
                    _patrolBehavior.Do();
                    _faceDir = _patrolBehavior.FaceDir;
                    if (EnemyBehaviorAction.XRayCastPlayer(transform.position, _faceDir, checkPlayerRange))
                    {
                        mFsm.SwitchState(DefaultEnemyState.Chase);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir);
                });

            mFsm.NewEventState(DefaultEnemyState.Chase)
                .OnEnter(() =>
                {
                    _chaseBehavior.Bind(_player.gameObject, moveController);
                })
                .OnFixedUpdate(() =>
                {
                    if (!EnemyBehaviorAction.XRayCastPlayer(transform.position, _faceDir, checkPlayerRange))
                    {
                        mFsm.SwitchState(DefaultEnemyState.Patrol);
                    }
                    //UnityAPIExt.DebugLineForRayCast2D(transform.position, Vector2.right * faceDir, checkPlayerRange, LayerMask.GetMask("Player"));

                    _faceDir = _chaseBehavior.FaceDir;
                    _chaseBehavior.Do();

                    if (_chaseBehavior.XCloseEnoughToTarget(checkAttackRange)) // can attack
                    {
                        mFsm.SwitchState(DefaultEnemyState.Attack);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir, 0.8f);
                });

            mFsm.NewEventState(DefaultEnemyState.Attack)
                .OnCustom(() => Attack(_player))
                .OnFixedUpdate(() =>
                {
                    if (!_chaseBehavior.XCloseEnoughToTarget(checkAttackRange)) // can attack
                    {
                        mFsm.SwitchState(DefaultEnemyState.Chase);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir, 0.8f);
                });

            mFsm.NewEventState(DefaultEnemyState.Dead);

            mFsm.StartState(DefaultEnemyState.Patrol);
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            _mRenderer.color = Color.white;
            _mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f)
                .OnComplete(() => _mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, 0.2f)
                .OnComplete(() => transform.position = initPos);
            gameObject.SetOnDestroy(() => _mRenderer.DOKill(), "mRenderer");
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }

        public override void Judge(int frame, bool force)
        {
            _mFsm.Custom();
        }
    }

    /*public class ZakoMachine : MonoBaseEnemy
    {
        // dependency
        private FSM<DefaultEnemyState> _fsm = new();
        private SmoothMoveCtrl moveController;
        private Player target;

        // parameter
        private readonly float attackMoveSpeedScale = 0.5f;
        private readonly float getPlayerYThresold = 2f;
        private readonly float moveSpeed = 100f;

        // behavior
        private SpriteRenderer mRenderer;
        private Collider2D touchDamageCollider;

        public LevelModel levelModel => GameplayService.Interface.LevelModel;

        protected void Awake()
        {
            mRenderer = GetComponentInChildren<SpriteRenderer>();
            touchDamageCollider = GetComponentInChildren<Collider2D>();
            moveController = GetComponent<SmoothMoveCtrl>();
            target = GameplayService.Interface.Get<Player>();

            InitializeFSM(_fsm);

            _health.Value = _property.healthPoint;
            if (_health.Value < 0)
            {
                _fsm.SwitchState(DefaultEnemyState.Dead);
                EnemyDieEvent.Trigger();
            }
        }

        private void OnDieBehavior()
        {
            touchDamageCollider.enabled = false;
            this.DefaultDieBehavior(moveController);
        }

        private void Attack()
        {
            var targetPos = target.transform.position;
            ActionCtrl.Delay(GameplayService.Interface.LevelModel.BpmIntervalInSeconds)
                .OnComplete(() =>
                {
                    moveController.XMove(moveSpeed * Mathf.Sign(targetPos.x - transform.position.x) * attackMoveSpeedScale);
                })
                .Start(gameObject);
        }

        protected void InitializeFSM(FSM<DefaultEnemyState> mFSM)
        {
            // Patrol: move around
            mFSM.NewEventState(DefaultEnemyState.Patrol)
                .OnFixedUpdate(() =>
                {

                });

            mFSM.NewEventState(DefaultEnemyState.Chase)
                .OnCustom(() =>
                {
                    // move
                    //var playerLoss = Mathf.Abs(target.transform.position.y - transform.position.y) < getPlayerYThresold;
                    moveController.XMove(moveSpeed * Mathf.Sign(target.transform.position.x - transform.position.x));
                    if (URandom.Range(0, 10) == 0)
                    {
                        moveController.JumpVelocity = _property.jumpForce;
                        moveController.SimpleJump();
                    }

                    // check
                    if (this.DefaultAttackCheck(_property.attackRange, target.transform.position))
                        _fsm.SwitchState(DefaultEnemyState.Attack);
                });

            mFSM.NewEventState(DefaultEnemyState.Attack)
                .OnCustom(() =>
                {
                    Attack();
                    if (!this.DefaultAttackCheck(_property.attackRange, target.transform.position))
                        _fsm.SwitchState(DefaultEnemyState.Chase);
                })
                .OnExit(() => transform.DOKill());

            mFSM.NewEventState(DefaultEnemyState.Dead)
                .OnEnter(() => OnDieBehavior())
                .OnCondition(() => mFSM.CurrentStateId != DefaultEnemyState.Dead);

            mFSM.StartState(DefaultEnemyState.Idle);
        }

        public override void OnHurt(int attackPoint)
        {
            if (_health.Value <= 0)
                return;
            this.DefaultHurtBehavior(mRenderer);
            this.DefaultHurtLogic(_health.Value - attackPoint, () => _fsm.SwitchState(DefaultEnemyState.Dead));
        }
        public override void OnJudge(int judgeCount)
        {
            _fsm.Custom();
        }
    }*/

    /*public class ZakoMachine : Enemy
    {
        protected override void RegisterState(FSM<EntityStateType> fsm)
        {
            fsm.AddState(EntityStateType.Spawn, new SpawnState(this));
            fsm.AddState(EntityStateType.Idle, new EnemyIdleState(this));
            fsm.AddState(EntityStateType.Attack, new CommonEnemyAttackState(this, Attack));
            fsm.AddState(EntityStateType.Dead, new DeadState(this));
        }

        private bool Attack(CommonEnemyAttackState state)
        {
            var player = GameplayService.Interface.Get<Player>();

            if (Vector3.Distance(player.transform.position, MoveController.Position) <= Property.attackRange)
                player.OnHurt(Property.attackPoint);
            return false;
        }
    }*/
}
