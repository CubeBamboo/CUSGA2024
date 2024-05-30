using CbUtils.Event;
using Shuile.Gameplay.Event;

using System;

using DG.Tweening;
using UnityEngine;

using UObject = UnityEngine.Object;
using Cysharp.Threading.Tasks;

namespace Shuile.Gameplay
{
    /// <summary> base class for enemy </summary>
    public abstract class Enemy : MonoBehaviour, IHurtable, IJudgeable
    {
        [SerializeField] protected int MaxHealth = 100;
        protected SmoothMoveCtrl moveController;
        protected int health;

        public int Health => health;
        public bool IsAlive => health > 0;
        public SmoothMoveCtrl MoveController => moveController;

        public event Action<int> OnHpChangedEvent = _ => { };
        private HUDHpBarElement hpBarUI;

        protected void Awake()
        {
            health = MaxHealth;
            moveController = GetComponent<SmoothMoveCtrl>();
            //MoveController.Ability = Property.moveAbility;
            //moveController.XMaxSpeed = Property.maxMoveSpeed;
            //moveController.Deceleration = Property.deceleration;

            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            if (hpBarUI) UObject.Destroy(hpBarUI.gameObject);
        }

        protected virtual void OnAwake() { }
        public virtual void OnHurt(int attackPoint)
        {
            if (Health <= 0)
                return;

            var oldVal = health;
            health = Mathf.Max(0, health - attackPoint);
            OnSelfHurt(oldVal, health);
            EnemyHurtEvent.Trigger(gameObject);

            if (Health == 0)
            {
                OnSelfDie();
                HandleDieEvent();
            }
        }
        private async void HandleDieEvent()
        {
            await UniTask.WaitUntil(() => !LevelEntityManager.Instance.IsJudging);
            EnemyDieEvent.Trigger(gameObject);
        }
        protected abstract void OnSelfHurt(int oldVal, int newVal);
        protected abstract void OnSelfDie();
        public abstract void Judge(int frame, bool force);
    }

    /*public abstract class Enemy : BehaviourEntity, IHurtable
    {
        [SerializeField] private EnemyPropertySO property;
        protected IMoveController moveController;
        protected int health;

        public EnemyPropertySO Property => property;
        public int Health => health;
        public bool IsAlive => health > 0;
        public IMoveController MoveController => moveController;

        public event Action<int> OnHpChangedEvent = _ => { };
        private HUDHpBarElement hpBarUI;
        private SpriteRenderer mRenderer;

        public Enemy() : base(EntityType.Enemy)
        {
        }

        protected override void Awake()
        {
            base.Awake();
            health = property.healthPoint;
            moveController = GetComponent<IMoveController>();
            //MoveController.Ability = Property.moveAbility;
            moveController.XMaxSpeed = Property.maxMoveSpeed;
            moveController.Deceleration = Property.deceleration;

            mRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected virtual void OnDestroy()
        {
            if (hpBarUI) UObject.Destroy(hpBarUI.gameObject);
        }

        public void OnHurt(int attackPoint)
        {
            if (health <= 0)
                return;

            // author: CubeBamboo
            // hurt FX
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                    transform.position = initPos);
            gameObject.SetOnDestroy(() => mRenderer.DOKill(), "mRenderer");
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
            // end

            health = Mathf.Max(0, health - attackPoint);
            // author: CubeBamboo
            OnHpChangedEvent(health);
            // end

            if (health == 0)
            {
                State = EntityStateType.Dead;
                EnemyDieEvent.Trigger();
            }
        }
    }*/
}
