using System;

namespace Shuile.Gameplay.Entity.States
{
    /// <summary>
    /// 攻击行为
    /// </summary>
    /// <returns>返回true表示下次判定继续攻击</returns>
    public delegate bool AttackBehaviour();

    public enum AttackStateType
    {
        PreAttack,
        Attack,
        PostAttack
    }

    public class CommonEnemyAttackState : EntityState
    {
        public readonly Enemy enemy;
        private AttackStateType attackState;
        private readonly AttackBehaviour attackBehaviour;
        private int counter;
        
        public CommonEnemyAttackState(BehaviourEntity entity, AttackBehaviour attackBehaviour) : base(entity)
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
        }

        public override void Judge()
        {
            if (attackState == AttackStateType.Attack)
            {
                var isContinue = attackBehaviour();
                if (!isContinue)
                {
                    AttackStateType attackStateType = enemy.Property.postAttackDuration != 0 ?
                                        AttackStateType.PostAttack :
                                        (enemy.Property.preAttackDuration != 0 ? AttackStateType.PreAttack : AttackStateType.Attack);
                    attackState = attackStateType;
                }

                return;
            }

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
