using CbUtils.Timing;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Event;
using Shuile.Gameplay.Feel;
using Shuile.Model;
using Shuile.Rhythm.Runtime;
using Shuile.Root;
using UnityEngine;
using UInput = UnityEngine.InputSystem;

namespace Shuile.Gameplay.Manager
{
    // if you dont know where to put the logic, put it here
    public class LevelGlobalManager : MonoSingletons<LevelGlobalManager>, IEntity
    {
        private LevelModel _levelModel;
        private MusicRhythmManager _musicRhythmManager;
        private LevelFeelManager _levelFeelManager;
        private LevelStateMachine _levelStateMachine;
        private Player _player;
        private PlayerModel _playerModel;

        protected override void OnAwake()
        {
            _levelFeelManager = this.GetUtility<LevelFeelManager>();
            _levelModel = this.GetModel<LevelModel>();
            _playerModel = this.GetModel<PlayerModel>();
            _levelStateMachine = this.GetSystem<LevelStateMachine>();
            _musicRhythmManager = MusicRhythmManager.Instance;
            _player = Player.Instance;
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
            var keyboard = UInput.Keyboard.current;
            
            if (keyboard.zKey.wasPressedThisFrame)
            {
                _musicRhythmManager.SetCurrentTime(_musicRhythmManager.CurrentTime + 5f);
            }
            if (keyboard.xKey.wasPressedThisFrame)
            {
                _musicRhythmManager.SetCurrentTime(_musicRhythmManager.CurrentTime - 5f);
            }
            
            if (keyboard.upArrowKey.isPressed && keyboard.downArrowKey.wasPressedThisFrame)
            {
                //DebugProperty.Instance.SetInt("PlayerKaiGua", 1);
                Debug.Log("开挂模式");
                _player.CurrentHp.Value = 999999;
            }
            if (keyboard.upArrowKey.isPressed && keyboard.leftArrowKey.wasPressedThisFrame)
            {
                _playerModel.canInviciable = !_playerModel.canInviciable;
                Debug.Log($"受击无敌变更 -> {_playerModel.canInviciable}");
            }
            if (keyboard.bKey.wasPressedThisFrame)
            {
                _player.OnHurt(20);
                _player.OnHurt((int)(_player.CurrentHp.Value * 0.25f));
            }
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
