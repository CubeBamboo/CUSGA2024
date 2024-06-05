using CbUtils.Timing;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Gameplay.Event;
using Shuile.Model;
using Shuile.Rhythm.Runtime;
using Shuile.Root;
using UnityEngine;

using UInput = UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    // if you dont know where to put the logic, put it here
    public class LevelGlobalManager : MonoSingletons<LevelGlobalManager>, IEntity
    {
        private LevelModel _levelModel;
        private MusicRhythmManager _musicRhythmManager;
        private LevelFeelManager _levelFeelManager;
        private LevelStateMachine _levelStateMachine;
        protected override void OnAwake()
        {
            _levelFeelManager = this.GetUtility<LevelFeelManager>();
            _levelModel = this.GetModel<LevelModel>();
            _levelStateMachine = this.GetSystem<LevelStateMachine>();
            _musicRhythmManager = MusicRhythmManager.Instance;
        }

        private void Start()
        {
            this.RegisterEvent<EnemyDieEvent>(GlobalOnEnemyDie);
            this.RegisterEvent<EnemyHurtEvent>(GlobalOnEnemyHurt);
        }
        private void OnDestroy()
        {
            this.UnRegisterEvent<EnemyDieEvent>(GlobalOnEnemyDie);
            this.UnRegisterEvent<EnemyHurtEvent>(GlobalOnEnemyHurt);

            TimingCtrl.Instance.StopAllTimer();
        }
        private void FixedUpdate()
        {
            if (!LevelRoot.Instance.IsStart) return;

            _levelModel.DangerScore -= DangerLevelConfigClass.NormalReductionPerSecond * Time.fixedDeltaTime;

            // check end
            if (_musicRhythmManager.IsMusicEnd)
            {
                _levelStateMachine.State = LevelStateMachine.LevelState.Win;
            }
        }
        private void Update()
        {
            DebugUpdate();
        }

        private void GlobalOnEnemyHurt(EnemyHurtEvent para)
        {
            _levelFeelManager.CameraShake();
            //MonoAudioCtrl.Instance.PlayOneShot("Enemy_Hurt");
        }
        private void GlobalOnEnemyDie(EnemyDieEvent para)
        {
            //MonoAudioCtrl.Instance.PlayOneShot("Enemy_Die");
        }

        private void DebugUpdate()
        {
            if (UInput.Keyboard.current.zKey.wasPressedThisFrame)
            {
                _musicRhythmManager.SetCurrentTime(_musicRhythmManager.CurrentTime + 5f);
            }
            if (UInput.Keyboard.current.xKey.wasPressedThisFrame)
            {
                _musicRhythmManager.SetCurrentTime(_musicRhythmManager.CurrentTime - 5f);
            }
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
