using CbUtils.Event;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Gameplay.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class MahouDefenseTower : Enemy
    {
        private static Transform bombParent;
        [SerializeField] private int attackPoint = 150;

        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private int explosionDelay = 4;
        [SerializeField] private int bombCount = 3;
        [SerializeField] private float explodeRadius = 4f;
        private readonly List<Bomb> bombs = new();

        private int counter;

        private SpriteRenderer mRenderer;

        public int ExplosionDelay
        {
            get => explosionDelay;
            set => explosionDelay = value;
        }

        public static Transform BombParent =>
            bombParent != null ? bombParent : bombParent = new GameObject("Bombs").transform;

        public bool IsDie { get; private set; }

        protected override void OnAwake()
        {
            mRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            moveController.IsFrozen = true;

            // enter anim
            EnterAnim().Forget();
        }

        private async UniTaskVoid EnterAnim()
        {
            transform.localScale = Vector3.one * 0.1f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.4f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.7f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 1f;
        }

        private async UniTask ExitAnim()
        {
            transform.localScale = Vector3.one * 0.7f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.4f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.1f;
        }

        public override void Judge(int frame, bool force)
        {
            Attack();
            counter++;
        }

        private void Attack()
        {
            if (counter == 0)
            {
                SpawnBombs();
            }

            if (counter < explosionDelay)
            {
                return;
            }

            foreach (var bomb in bombs)
            {
                bomb.Explode(attackPoint, explodeRadius);
            }

            bombs.Clear();
            counter = -1;
        }

        private void SpawnBombs()
        {
            for (var i = 0; i < bombCount; i++)
            {
                var pos = LevelZoneManager.Instance.RandomValidPosition();
                var bomb = Instantiate(bombPrefab, pos, Quaternion.identity, BombParent).GetComponent<Bomb>();
                bombs.Add(bomb);
            }
        }

        private void InterruptAttack()
        {
            foreach (var bomb in bombs)
            {
                bomb.Interrupt();
            }

            bombs.Clear();
        }

        protected override async void OnSelfDie()
        {
            IsDie = true;
            InterruptAttack();

            await ExitAnim();
            Destroy(gameObject);
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f)
                .OnComplete(() => mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, 0.2f)
                .OnComplete(() => transform.position = initPos);
            gameObject.SetOnDestroy(() => mRenderer.DOKill(), "mRenderer");
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }
    }

    /*public class MahouDefenseTower : MonoStateableEnemy<DefaultEnemyState>
    {
        private SpriteRenderer mRenderer;

        private void Awake()
        {
            mRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void InitializeFSM(FSM<DefaultEnemyState> mFSM)
        {
            throw new System.NotImplementedException();
        }
        public abstract void OnJudge(int judgeCount)
        {
            if(IsLoadEnd)
                _fsm.Custom();
        }

        public override void OnHurt(int attackPoint)
        {
            if (health.Value <= 0) return;
            PlayHurtBehavior();
            DefaultHurtLogic(health.Value - attackPoint, () => State = DefaultEnemyState.Dead);
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


    /*public class MahouDefenseTower : Enemy
    {
        [SerializeField] private int explosionDelay = 4;
        [SerializeField] private int bombCount = 3;
        [SerializeField] private float explodeRadius = 4f;
        [SerializeField] private GameObject bombPrefab;
        private static Transform bombParent;
        private List<Bomb> bombs = new();

        public int ExplosionDelay
        {
            get => explosionDelay;
            set => explosionDelay = value;
        }

        public static Transform BombParent
        {
            get
            {
                if (bombParent == null)
                    bombParent = new GameObject("Bombs").transform;
                return bombParent;
            }
        }

        //protected override void RegisterState(FSM<EntityStateType> fsm)
        //{
        //    fsm.AddState(EntityStateType.Spawn, new SpawnState(this));
        //    var attackState = new CommonEnemyAttackState(this, Attack, InterruptAttack);
        //    fsm.AddState(EntityStateType.Idle, attackState);
        //    fsm.AddState(EntityStateType.Attack, attackState);
        //    fsm.AddState(EntityStateType.Dead, new DeadState(this));
        //}

        //private bool Attack()
        //{
        //    if (state.counter == 1)
        //    {
        //        for (var i = 0; i < bombCount; i++)
        //        {
        //            var pos = LevelZoneManager.Instance.RandomValidPosition();
        //            var bomb = Instantiate(bombPrefab, pos, Quaternion.identity, BombParent).GetComponent<Bomb>();
        //            bombs.Add(bomb);
        //        }
        //    }
        //    if (state.counter < explosionDelay)
        //        return true;

        //    foreach (var bomb in bombs)
        //        bomb.Explode(Property.attackPoint, explodeRadius);

        //    bombs.Clear();
        //    return false;
        //}

        private void InterruptAttack()
        {
            foreach (var bomb in bombs)
                bomb.Interrupt();
            bombs.Clear();
        }

        public override void Judge(int frame, bool force)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnSelfDie()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            throw new System.NotImplementedException();
        }
    }*/
}
