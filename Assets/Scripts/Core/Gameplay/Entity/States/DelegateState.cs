namespace Shuile.Gameplay.Entity.States
{
    public delegate void OnJudge();
    public delegate void OnStateEnter();
    public delegate void OnStateExit();

    public class DelegateState : EntityState
    {
        private OnJudge judgeAction;
        private OnStateEnter enterAction;
        private OnStateExit exitAction;

        public DelegateState(BehaviourEntity entity) : base(entity)
        {
        }

        public DelegateState(BehaviourEntity entity, OnJudge judgeAction, OnStateEnter enterAction = null, OnStateExit exitAction = null) : base(entity)
        {
            this.judgeAction = judgeAction;
            this.enterAction = enterAction;
            this.exitAction = exitAction;
        }

        public override void Judge()
        {
            judgeAction?.Invoke();
        }

        public override void EnterState()
        {
            enterAction?.Invoke();
        }

        public override void ExitState()
        {
            exitAction?.Invoke();
        }
    }
}
