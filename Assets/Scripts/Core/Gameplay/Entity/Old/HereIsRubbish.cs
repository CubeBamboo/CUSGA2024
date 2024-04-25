/************************************************
 * author: CubeBamboo
 * 
 * here are some rubbish code i wrote in the past. i put them here for commemorate
 * 
 * Latest Update: 2024.4.26 02:35
 ************************************************/

/*
namespace Shuile.Gameplay.Entity.Enemies
{
    public class ZakoPatrolState : BaseState<DefaultEnemyState, Enemy>
    {
        private Player player;
        private float faceDir;

        private float acceleration = 3f;
        private float attackRange = 2f;

        public ZakoPatrolState(FSM<DefaultEnemyState> fsm, Enemy target) : base(fsm, target)
        {
        }

        public void Bind()
        {

        }

        public override void Enter()
        {
            player = GameplayService.Interface.Get<Player>();
        }
        public override void FixedUpdate()
        {
            // patrol move
            // move to faceDir, if meet edge, change faceDir, if meet wall, jump
            if (target.MoveController.IsOnGround && CheckWall(target.transform.position))
                target.MoveController.SimpleJump(1f);

            // move to path
            var dir = Mathf.Sign((player.transform.position - target.transform.position).x);
            target.MoveController.XMove(dir * target.Property.acceleration);
            faceDir = dir;

            // check find player
            if (CheckFindPlayer(target.Property.attackRange, target.transform.position))
                fsm.SwitchState(DefaultEnemyState.Chase);
        }
        public override void Custom()
        {
            var playerPos = GameplayService.Interface.Get<Player>().transform.position;
            var dst = Vector3.Distance(playerPos, target.transform.position);

            if (dst > target.Property.attackRange)
                return;

            fsm.SwitchState(DefaultEnemyState.Attack);
        }

        private bool CheckWall(Vector2 startPos)
            => Physics2D.Raycast(startPos, new Vector2(faceDir, 0), 0.3f, LayerMask.GetMask("Ground"));
        private bool CheckFindPlayer(float maxDistance, Vector2 startPosition)
            => Physics2D.Raycast(startPosition, new Vector2(faceDir, 0), maxDistance, LayerMask.GetMask("Player"));
    }

    public class ZakoMachine : Enemy
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
    }
}
*/


/*namespace Shuile.Gameplay.Entity
{
    /// <summary>
    /// refactor version of Enemy.cs
    /// provides some base methods use for enemy.
    /// for example, finite state machine initialization
    /// </summary>
    public abstract class MonoBaseEnemy : MonoBehaviour, IHurtable
    {
        [SerializeField] protected EnemyPropertySO _property;
        protected HearableProperty<int> _health = new();

        public EnemyPropertySO Property => _property;
        public HearableProperty<int> Health => _health;

        /// <summary> auto trigger by manager </summary>
        public abstract void OnJudge(int judgeCount);
        public abstract void OnHurt(int attackPoint);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
                if (collision.TryGetComponent<Player>(out var player))
                    player.OnHurt(Property.attackPoint);
        }
    }

    // it's strange...
    [System.Obsolete()]
    public abstract class MonoStateableEnemy<TState> : MonoBaseEnemy
    {
        protected FSM<TState> _fsm = new();
        public TState State
        {
            get => _fsm.CurrentStateId;
            set
            {
                if (value.Equals(_fsm.CurrentStateId))
                    return;
                _fsm.SwitchState(value);
            }
        }

        public bool IsLoadEnd { get; protected set; } = false;

        protected void Awake()
        {
            InitializeFSM(_fsm);
            OnAwake();
            IsLoadEnd = true;
        }
        protected virtual void OnAwake() { }
        /// <summary> auto call in Awake() </summary>
        protected abstract void InitializeFSM(FSM<TState> mFSM);

        private SmoothMoveCtrl _moveCtrl;
        private Player _target;
        public SmoothMoveCtrl moveController => _moveCtrl ??= GetComponent<SmoothMoveCtrl>();
        public Player target => _target = _target ? _target : GameplayService.Interface.Get<Player>();

        /// <summary> distance less than _property.attackRange </summary>
        protected bool DefaultAttackCheck()
            => (target.transform.position - transform.position).sqrMagnitude < _property.attackRange * _property.attackRange;
        protected void DefualtDieBehavior()
        {
            moveController.IsFrozen = true;
            transform.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => Object.Destroy(gameObject));
        }
    }
}*/

/*namespace Shuile.Gameplay.Entity
{
    public static class MonoEnemyExtension
    {
        // not nessary... use extension more for some complex logic
        public static void DefaultHurtBehavior(this MonoBaseEnemy enemy, SpriteRenderer mRenderer)
        {
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                           mRenderer.DOColor(Color.white, 0.2f));
            var initPos = enemy.transform.position;
            enemy.transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                               enemy.transform.position = initPos);
            enemy.gameObject.SetOnDestroy(() => mRenderer.DOKill(), "mRenderer");
            enemy.gameObject.SetOnDestroy(() => enemy.transform.DOKill(), "transform");
        }
        public static bool DefaultAttackCheck(this MonoBaseEnemy enemy, float attackRadius, Vector3 target)
            => (target - enemy.transform.position).sqrMagnitude < attackRadius * attackRadius;
        public static void DefaultDieBehavior(this MonoBaseEnemy enemy, SmoothMoveCtrl moveCtrl)
        {
            moveCtrl.IsFrozen = true;
            enemy.transform.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => Object.Destroy(enemy.gameObject));
        }
        public static void DefaultHurtLogic(this MonoBaseEnemy enemy, int newValue, System.Action onDie)
        {
            if (enemy.Health.Value <= 0)
                return;

            enemy.Health.Value = Mathf.Max(0, newValue);

            if (enemy.Health.Value == 0)
            {
                onDie?.Invoke();
                EnemyDieEvent.Trigger(enemy.gameObject);
            }
        }

    }
}*/
