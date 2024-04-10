using CbUtils;

namespace Shuile.Gameplay
{
    public abstract class EntityState : IState
    {
        protected BehaviourEntity entity;

        public EntityState(BehaviourEntity entity)
        {
            this.entity = entity;
        }

        public virtual bool Condition() => true;

        public void Custom(string label = "") => Judge();

        public abstract void Judge();

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnGUI()
        {
        }

        public virtual void Update()
        {
        }
    }
}
