using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay
{
    public abstract class Enemy : BehaviourEntity, IAttackable
    {
        [SerializeField] private EnemyPropertySO property;
        private int health;

        public int Health => health;
        public EnemyPropertySO Property => property;
        public bool IsAlive => health > 0;

        protected override void Awake()
        {
            base.Awake();
            health = property.healthPoint;
        }

        public void OnAttack(int attackPoint)
        {
            if (health <= 0)
                return;

            // hurt FX
            // author: CubeBamboo
            SpriteRenderer mRenderer = GetComponentInChildren<SpriteRenderer>();
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                    transform.position = initPos);
            // end

            health = Mathf.Max(0, health - attackPoint);
            if (health == 0)
            {
                GotoState(EntityStateType.Dead);
            }
        }
    }
}
