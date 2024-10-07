using CbUtils;
using CbUtils.Event;
using CbUtils.Extension;
using DG.Tweening;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class Creeper : Enemy
    {
        [SerializeField] private float jumpVel = 10f;
        [SerializeField] private float xMaxSpeed = 5f;
        [SerializeField] private float checkPlayerRange = 5f;
        [SerializeField] private float checkAttackRange = 1.2f;
        [SerializeField] private float attackRange = 2f;
        private readonly FSM<DefaultEnemyState> _mFsm = new();
        private ZakoChaseBehavior _chaseBehavior;
        private float _faceDir = 1f;

        private SpriteRenderer _mRenderer;

        private ZakoPatrolBehavior _patrolBehavior;

        public override void Awake()
        {
            base.Awake();
            RegisterState(_mFsm);

            _patrolBehavior = new ZakoPatrolBehavior(gameObject, moveController, 5f);
            _chaseBehavior = new ZakoChaseBehavior();
            _mRenderer = GetComponentInChildren<SpriteRenderer>();
            moveController.JumpVelocity = jumpVel;
            moveController.XMaxSpeed = xMaxSpeed;
        }

        private void Update()
        {
            _mFsm.Update();
        }

        private void FixedUpdate()
        {
            _mFsm.FixedUpdate();
        }

        protected override void OnSelfDie()
        {
            _mFsm.SwitchState(DefaultEnemyState.Dead);
            moveController.IsFrozen = true;
            transform.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => gameObject.Destroy());
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

                    UnityAPIExtension.DebugLineForRayCast2D(transform.position, Vector2.right * _faceDir,
                        checkPlayerRange, LayerMask.GetMask("Player"));

                    _faceDir = _chaseBehavior.FaceDir;
                    _chaseBehavior.Do();

                    if (_chaseBehavior.XCloseEnoughToTarget(checkAttackRange))
                    {
                        mFsm.SwitchState(DefaultEnemyState.Attack);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir);
                });
            mFsm.NewEventState(DefaultEnemyState.Attack)
                .OnCustom(() => Attack());
            mFsm.NewEventState(DefaultEnemyState.Dead)
                .OnEnter(() => OnHurt(MaxHealth));

            mFsm.StartState(DefaultEnemyState.Patrol);
        }

        private bool Attack()
        {
            if (Vector3.Distance(_player.transform.position, MoveController.Position) <= attackRange)
            {
                _player.OnHurt((int)(_player.Property.maxHealthPoint * 0.6f));
            }

            _mFsm.SwitchState(DefaultEnemyState.Dead);
            return true;
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

    //protected void RegisterState(FSM<DefaultEnemyState> mFsm)
    //{
    //    //mFsm.AddState(EntityStateType.Spawn, new SpawnState(this));
    //    //mFsm.AddState(EntityStateType.Idle, new EnemyIdleState(this));
    //    //mFsm.AddState(EntityStateType.Attack, new CommonEnemyAttackState(this, Attack));
    //    //mFsm.AddState(EntityStateType.Dead, new DeadState(this));
    //}
}

/*public class Creeper : MonoStateableEnemy<DefaultEnemyState>
{
    private readonly float moveSpeed = 20f;
    private readonly float explodeRadius = 4f;
    private Collider2D touchDamageCollider;
    private SpriteRenderer mRenderer;

    private int attackCounter = 0;
    protected override void OnAwake()
    {
        touchDamageCollider = GetComponentInChildren<Collider2D>();
        mRenderer = GetComponentInChildren<SpriteRenderer>();
        attackCounter = 0;
    }

    private void OnDieBehavior()
    {
        touchDamageCollider.enabled = false;
        DefualtDieBehavior();
    }
    private void Attack()
    {
        // behavior
        mRenderer.DOColor(Color.red, 0.15f).OnComplete(() => mRenderer.DOColor(Color.white, 0.15f));

        // logic
        if (attackCounter <= 0)
        {
            attackCounter++;
            return;
        }

        if (Vector3.Distance(target.transform.position, moveController.Position) <= explodeRadius)
            target.OnHurt((int)(target.Property.maxHealthPoint * 0.6f));
        State = DefaultEnemyState.Dead;
    }

    private void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }

    protected override void InitializeFSM(FSM<DefaultEnemyState> mFSM)
    {
        mFSM.NewEventState(DefaultEnemyState.Move)
            .OnCustom(() =>
            {
                if (URandom.Range(0, 10) == 0)
                {
                    moveController.JumpVelocity = _property.jumpForce;
                    moveController.SimpleJump();
                }

                if (Vector3.Distance(target.transform.position, moveController.Position) <= Property.attackRange)
                    State = DefaultEnemyState.Attack;
            })
            .OnFixedUpdate(() =>
            {
                moveController.XMove(moveSpeed * Mathf.Sign(target.transform.position.x - transform.position.x));
            });

        mFSM.NewEventState(DefaultEnemyState.Attack)
            .OnCustom(() =>
            {
                moveController.IsFrozen = true;
                Attack();
            });
        mFSM.NewEventState(DefaultEnemyState.Dead)
            .OnEnter(() => OnDieBehavior())
            .OnCondition(() => mFSM.CurrentStateId != DefaultEnemyState.Dead);

        mFSM.StartState(DefaultEnemyState.Move);
    }

    public override void OnJudge(int judgeCount)
    {
        if(IsLoadEnd)
            _fsm.Custom();
    }

    public override void OnHurt(int attackPoint)
    {
        if(_health.Value <= 0) return;
        PlayHurtBehavior();
        this.DefaultHurtLogic(_health.Value - attackPoint, () => State = DefaultEnemyState.Dead);
    }

    private void PlayHurtBehavior()
    {
        mRenderer.color = Color.white;
        mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
            mRenderer.DOColor(Color.white, 0.2f));
        var initPos = transform.position;
        transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                transform.position = initPos);
        gameObject.SetOnDestroy(() => mRenderer.DOKill(), "mRenderer");
        gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
    }

}*/
