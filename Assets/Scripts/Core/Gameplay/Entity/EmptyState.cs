namespace Shuile.Gameplay.Entity
{
    public class EmptyState : EntityState
    {
        public static readonly EmptyState instance = new(null);

        public EmptyState(BehaviourEntity entity) : base(null)
        {
        }

        public override void EnterState() { }
        public override void ExitState() { }
        public override void Judge() { }
    }
}
