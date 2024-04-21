using CbUtils.Event;
using CbUtils.Extension;
using DG.Tweening;

using System;

using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
            this.interruptBehaviour = interruptBehaviour;
        }

        public override void Enter()
        {
            attackState = AttackStateType.PreAttack;
            counter = 0;
            // attackState = enemy.Property.preAttackDuration == 0 ? AttackStateType.Attack : AttackStateType.PostAttack;
        }

        public override void Exit()
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
                    NextState();

                return;
            }

            var s = DOTween.Sequence();
            var spriteRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
            s.Append(spriteRenderer.DOColor(Color.yellow, 0.1f));
            s.Append(spriteRenderer.DOColor(Color.white, 0.1f));
            s.Play();
            enemy.gameObject.SetOnDestroy(() => spriteRenderer.DOKill());

            var endCount = attackState == AttackStateType.PreAttack ? enemy.Property.preAttackDuration : enemy.Property.postAttackDuration;
            if (++counter >= endCount)
                NextState();
        }

        private void NextState()
        {
            counter = 0;
            if (attackState == AttackStateType.Attack)
            {
                attackState = AttackStateType.PostAttack;
                if (enemy.Property.postAttackDuration != 0)
                    return;
            }
            if (attackState == AttackStateType.PostAttack)
            {
                var player = GameplayService.Interface.Get<Player>();
                var gridDistance = Vector3.Distance(player.transform.position, enemy.MoveController.Position);

                if (gridDistance > ((Enemy)entity).Property.attackRange)
                {
                    enemy.State = EntityStateType.Idle;
                    return;
                }
                attackState = AttackStateType.PreAttack;
                if (enemy.Property.preAttackDuration != 0)
                    return;
            }
            attackState = AttackStateType.Attack;
        }
    }
}
