using System.Runtime.CompilerServices;

namespace Shuile.Gameplay.EnemyState
{
    public abstract class State
    {
        private Enemy enemy;

        protected Enemy BindEnemy => enemy;

        public State()
        {
        }

        /// <summary>
        /// 该判定了
        /// </summary>
        public abstract void Judge();

        public virtual void EnterState() { }

        public virtual void ExitState() { }

        public virtual void Rebind(Enemy newEnemy)
        {
            enemy = newEnemy;
        }

        /// <summary>
        /// A shortcut for BindEnemy.GotoState(State newState);
        /// </summary>
        /// <param name="newState"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void GotoState(State newState) => enemy.GotoState(newState);
    }
}
