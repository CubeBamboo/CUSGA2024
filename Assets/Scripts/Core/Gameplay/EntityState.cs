using System;
using System.Runtime.CompilerServices;

namespace Shuile.Gameplay
{
    public delegate void OnEntityStateChanged(EntityStateType from, EntityStateType to);

    [Obsolete("TODO: Use FSM", false)]
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
        public virtual void EnterState() { }
        public virtual void ExitState() { }

        /// <summary>
        /// A shortcut for BindEnemy.GotoState(State newState);
        /// </summary>
        /// <param name="state">new state</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void GotoState(EntityStateType state) => entity.GotoState(state);
    }
}
