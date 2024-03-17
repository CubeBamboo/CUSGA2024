namespace Shuile.Gameplay.EnemyState
{
    public abstract class State
    {
        protected readonly Enemy enemy;

        public State(Enemy enemy)
        {
            this.enemy = enemy;
        }

        /// <summary>
        /// 该判定了
        /// </summary>
        public abstract void Judge();

        public virtual void EnterState() { }

        public virtual void ExitState() { }
    }
}
