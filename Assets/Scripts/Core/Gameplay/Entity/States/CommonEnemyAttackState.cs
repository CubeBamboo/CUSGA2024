using DG.Tweening;

using System;

using UnityEngine;

namespace Shuile.Gameplay.Entity.States
{
    public enum AttackStateType
    {
        PreAttack,
        Attack,
        PostAttack
    }

    public class CommonEnemyAttackState : EntityState
    {
        /// <summary>
        /// 攻击被打断
        /// </summary>
        /// <param name="state"></param>
        public delegate void InterruptBehaviour(CommonEnemyAttackState state);

        /// <summary>
        /// 攻击行为
        /// </summary>
        /// <returns>返回true表示下次判定继续攻击</returns>
        public delegate bool AttackBehaviour(CommonEnemyAttackState state);

        public readonly Enemy enemy;
        private AttackStateType attackState;
        private readonly AttackBehaviour attackBehaviour;
        private readonly InterruptBehaviour interruptBehaviour;
        public int counter;
        
        public CommonEnemyAttackState(BehaviourEntity entity,
            AttackBehaviour attackBehaviour,
            InterruptBehaviour interruptBehaviour = null) : base(entity)
        {
            if (entity is not Enemy)
                throw new InvalidCastException($"entity is {entity.GetType()} not {nameof(Enemy)}");
            enemy = (Enemy)entity;
            this.attackBehaviour = attackBehaviour;
        }

        public override void EnterState()
        {
            attackState = AttackStateType.PreAttack;
            counter = 0;
            attackState = enemy.Property.preAttackDuration == 0 ? AttackStateType.Attack : AttackStateType.PostAttack;
        }

        public override void ExitState()
        {
            if (attackState == AttackStateType.Attack && counter != 0)
                interruptBehaviour?.Invoke(this);
        }

        public override void Judge()
        {
            ++counter;
            if (attackState == AttackStateType.Attack)
            {
                var isContinue = attackBehaviour(this);
                if (!isContinue)
                {
                    AttackStateType attackStateType = enemy.Property.postAttackDuration != 0 ?
                                        AttackStateType.PostAttack :
                                        (enemy.Property.preAttackDuration != 0 ? AttackStateType.PreAttack : AttackStateType.Attack);
                    attackState = attackStateType;
                    counter = 0;
                }

                return;
            }

            var s = DOTween.Sequence();
            var spriteRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
            s.Append(spriteRenderer.DOColor(Color.yellow, 0.1f));
            s.Append(spriteRenderer.DOColor(Color.white, 0.1f));
            s.Play();

            var endCount = attackState == AttackStateType.PreAttack ? enemy.Property.preAttackDuration : enemy.Property.postAttackDuration;
            if (++counter >= endCount)
            {
                counter = 0;
                attackState = attackState == AttackStateType.PreAttack || (attackState == AttackStateType.PostAttack && enemy.Property.preAttackDuration == 0) ?
                    AttackStateType.Attack : AttackStateType.PreAttack;
            }
        }
    }
}
