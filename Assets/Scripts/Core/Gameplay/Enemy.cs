using DG.Tweening;
using Shuile.Framework;
using UnityEngine;

namespace Shuile.Gameplay
{
    public abstract class Enemy : BehaviourEntity, IHurtable
    {
        [SerializeField] private EnemyPropertySO property;
        private int health;

        public int Health => health;
        public EnemyPropertySO Property => property;
        public bool IsAlive => health > 0;

        public event System.Action<int> OnHpChangedEvent = _ => { };
        private HUDHpBarElement hpBarUI;

        protected override void Awake()
        {
            base.Awake();
            health = property.healthPoint;
            MoveController.MaxSpeed = Property.maxMoveSpeed;
            MoveController.Deceleration = Property.deceleration;

            // author: CubeBamboo
            hpBarUI = UICtrl.Instance.Create<HUDHpBarElement>();
            hpBarUI.Link(this).Show();
            // end
        }

        private void OnDestroy()
        {
            if(hpBarUI) Object.Destroy(hpBarUI.gameObject);
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
                GotoState(EntityStateType.Dead);
            }
        }
    }
}
