using Shuile.Core;
using Shuile.Rhythm.Runtime;

namespace Shuile.Gameplay
{
    public abstract class Gadget : BehaviourEntity
    {
        private IMusicRhythmManager _musicRhythmManager;

        public float destroyTime = float.PositiveInfinity;

        public Gadget() : base(EntityType.Gadget)
        {
            _musicRhythmManager = GameApplication.ServiceLocator.GetService<IMusicRhythmManager>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_musicRhythmManager.CurrentTime >= destroyTime)
            {
                fsm.SwitchState(EntityStateType.Dead);
            }
        }
    }
}
