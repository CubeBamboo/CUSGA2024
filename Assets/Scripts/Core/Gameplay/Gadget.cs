//using Shuile.Core;
//using Shuile.Rhythm.Runtime;

//namespace Shuile.Gameplay
//{
//    public abstract class Gadget : BehaviourLevelEntity
//    {
//        private MusicRhythmManager _musicRhythmManager;

//        public float destroyTime = float.PositiveInfinity;

//        public Gadget() : base(LevelEntityType.Gadget)
//        {
//            _musicRhythmManager = GameApplication.ServiceLocator.GetService<MusicRhythmManager>();
//        }

//        protected override void FixedUpdate()
//        {
//            base.FixedUpdate();

//            if (_musicRhythmManager.CurrentTime >= destroyTime)
//            {
//                fsm.SwitchState(EntityStateType.Dead);
//            }
//        }
//    }
//}
