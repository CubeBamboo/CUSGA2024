using CbUtils;
using CbUtils.Event;
using CbUtils.Extension;
using DG.Tweening;
using Shuile.Gameplay.Character;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class Creeper : Enemy
    {
        private FSM<DefaultEnemyState> mFsm = new();
        private Player player;

        ZakoPatrolBehavior patrolBehavior;
        ZakoChaseBehavior chaseBehavior;

        [SerializeField] private float jumpVel = 10f;
        [SerializeField] private float xMaxSpeed = 5f;
        [SerializeField] private float checkPlayerRange = 5f;
        [SerializeField] private float checkAttackRange = 1.2f;
        [SerializeField] private float attackRange = 2f;

        float faceDir = 1f;
        private SpriteRenderer mRenderer;

        protected override void OnAwake()
        {
            moveController.JumpVelocity = jumpVel;
            moveController.XMaxSpeed = xMaxSpeed;
            patrolBehavior = new(gameObject, moveController, 5f);
            chaseBehavior = new();
            mRenderer = GetComponentInChildren<SpriteRenderer>();
            
            RegisterState(mFsm);
        }

        private void Start()
        {
            player = Player.Instance;
        }

        protected override void OnSelfDie()
        {
            mFsm.SwitchState(DefaultEnemyState.Dead);
            moveController.IsFrozen = true;
            transform.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => gameObject.Destroy());
        }

        protected void RegisterState(FSM<DefaultEnemyState> mFsm)
        {
            mFsm.NewEventState(DefaultEnemyState.Patrol)
                .OnFixedUpdate(() =>
                {
                    patrolBehavior.Do();
                    faceDir = patrolBehavior.FaceDir;
                    if (EnemyBehaviorAction.XRayCastPlayer(transform.position, faceDir, checkPlayerRange))
                        mFsm.SwitchState(DefaultEnemyState.Chase);

                    EnemyBehaviorAction.CheckWallAndJump(moveController, faceDir);
                });
            mFsm.NewEventState(DefaultEnemyState.Chase)
                .OnEnter(() =>
                {
                    chaseBehavior.Bind(player.gameObject, moveController);
                })
                .OnFixedUpdate(() =>
                {
                    if (!EnemyBehaviorAction.XRayCastPlayer(transform.position, faceDir, checkPlayerRange))
                        mFsm.SwitchState(DefaultEnemyState.Patrol);
                    UnityAPIExtension.DebugLineForRayCast2D(transform.position, Vector2.right * faceDir, checkPlayerRange, LayerMask.GetMask("Player"));

                    faceDir = chaseBehavior.FaceDir;
                    chaseBehavior.Do();

                    if(chaseBehavior.XCloseEnoughToTarget(checkAttackRange))
                        mFsm.SwitchState(DefaultEnemyState.Attack);
                    EnemyBehaviorAction.CheckWallAndJump(moveController, faceDir);
                });
            mFsm.NewEventState(DefaultEnemyState.Attack)
                .OnCustom(() => Attack());
            mFsm.NewEventState(DefaultEnemyState.Dead)
                .OnEnter(() => OnHurt(MaxHealth));

            mFsm.StartState(DefaultEnemyState.Patrol);
        }

        private bool Attack()
        {
            var player = Player.Instance;

            if (Vector3.Distance(player.transform.position, MoveController.Position) <= attackRange)
                player.OnHurt((int)(player.Property.maxHealthPoint * 0.6f));

            mFsm.SwitchState(DefaultEnemyState.Dead);
            return true;
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f)
                     .OnComplete(() => mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f)
                     .OnComplete(() => transform.position = initPos);
            gameObject.SetOnDestroy(() => mRenderer.DOKill(), "mRenderer");
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }

        public override void Judge(int frame, bool force)
            => mFsm.Custom();
        private void Update()
            => mFsm.Update();
        private void FixedUpdate()
            => mFsm.FixedUpdate();
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
