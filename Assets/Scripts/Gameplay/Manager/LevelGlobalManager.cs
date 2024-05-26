using CbUtils.Timing;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Event;
using Shuile.Model;
using Shuile.Rhythm.Runtime;
using Shuile.Root;
using UnityEngine;

using UInput = UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    // if you dont know where to put the logic, put it here
    public class LevelGlobalManager : MonoEntity
    {
        private MusicRhythmManager _musicRhythmManager;
        private LevelModel levelModel;
        protected override void AwakeOverride()
        {
            levelModel = this.GetModel<LevelModel>();
            _musicRhythmManager = this.GetSystem<MusicRhythmManager>();
        }

        private void Start()
        {
            EnemyDieEvent.Register(GlobalOnEnemyDie);
            EnemyHurtEvent.Register(GlobalOnEnemyHurt);
        }
        protected override void OnDestroyOverride()
        {
            EnemyDieEvent.UnRegister(GlobalOnEnemyDie);
            EnemyHurtEvent.UnRegister(GlobalOnEnemyHurt);

            TimingCtrl.Instance.StopAllTimer();
        }
        private void FixedUpdate()
        {
            if (!LevelRoot.Instance.IsStart) return;

            levelModel.DangerScore -= DangerLevelConfigClass.NormalReductionPerSecond * Time.fixedDeltaTime;

            // check end
            if (_musicRhythmManager.IsMusicEnd)
            {
                LevelStateMachine.Instance.State = LevelStateMachine.LevelState.Win;
            }
        }

        private void GlobalOnEnemyHurt(GameObject @object)
        {
            LevelFeelManager.Instance.CameraShake();
            //MonoAudioCtrl.Instance.PlayOneShot("Enemy_Hurt");
        }
        private void GlobalOnEnemyDie(GameObject go)
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

        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
