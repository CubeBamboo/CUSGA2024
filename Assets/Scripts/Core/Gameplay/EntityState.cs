using System.Runtime.CompilerServices;

namespace Shuile.Gameplay
{
    public abstract class EntityState
    {
        protected BehaviourEntity entity;

        public EntityState(BehaviourEntity entity)
        {
            this.entity = entity;
        }

        /// <summary>
        /// 该判定了
        /// </summary>
        public abstract void Judge();
        public abstract void EnterState();
        public abstract void ExitState();

        /// <summary>
        /// A shortcut for BindEnemy.GotoState(State newState);
        /// </summary>
        /// <param name="state">new state</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void GotoState(EntityStateType state) => entity.GotoState(state);
    }
}
