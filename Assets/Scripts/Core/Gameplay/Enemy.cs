using DG.Tweening;

using Shuile.Framework;

using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay
{
    public abstract class Enemy : BehaviourEntity, IHurtable
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

        public Enemy() : base(EntityType.Enemy)
        {
        }

        protected override void Awake()
        {
            base.Awake();
            health = property.healthPoint;
            moveController = GetComponent<IMoveController>();
            MoveController.Ability = Property.moveAbility;
            moveController.MaxSpeed = Property.maxMoveSpeed;
            moveController.Deceleration = Property.deceleration;

            // author: CubeBamboo
            hpBarUI = UICtrl.Instance.Create<HUDHpBarElement>();
            hpBarUI.Link(this).Show();
            // end

            fsm.StartState(EntityStateType.Spawn);
            fsm.Custom();
        }

        protected virtual void OnDestroy()
        {
            if(hpBarUI) UObject.Destroy(hpBarUI.gameObject);
        }

        public void OnAttack(int attackPoint)
        {
            if (health <= 0)
                return;

            // author: CubeBamboo
            // hurt FX
            SpriteRenderer mRenderer = GetComponentInChildren<SpriteRenderer>();
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                    transform.position = initPos);
            // end

            health = Mathf.Max(0, health - attackPoint);
            // author: CubeBamboo
            OnHpChangedEvent(health);
            // end

            if (health == 0)
            {
                State = EntityStateType.Dead;
            }
        }
    }
}
