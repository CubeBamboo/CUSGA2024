using Shuile.Rhythm;

namespace Shuile.Gameplay
{
    public abstract class Gadget : BehaviourEntity
    {
        public float destroyTime = float.PositiveInfinity;

        public Gadget() : base(EntityType.Gadget)
        {
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (MusicRhythmManager.Instance.CurrentTime >= destroyTime)
            {
                fsm.SwitchState(EntityStateType.Dead);
            }
        }
    }
}