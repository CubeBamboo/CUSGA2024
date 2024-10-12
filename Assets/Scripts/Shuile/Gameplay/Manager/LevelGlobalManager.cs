using CbUtils.Kits.Tasks;
using CbUtils.Timing;
using Shuile.Core.Framework;
using Shuile.Core.Gameplay;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Event;
using Shuile.Gameplay.Feel;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.UI.Gameplay;
using Shuile.Utils;
using System;
using System.Collections;
using UnityEngine;
using UInput = UnityEngine.InputSystem;

namespace Shuile.Gameplay.Manager
{
    // if you dont know where to put the logic, put it here
    public class LevelGlobalManager : MonoContainer
    {
        private GamePlayScene _gamePlayScene;
        private Player _player;
        private AutoPlayChartManager _autoPlayChartManager;
        private EndLevelPanel _endLevelPanel;
        private LevelEntityManager _levelEntityManager;
        private LevelFeelManager _levelFeelManager;
        private LevelModel _levelModel;
        private LevelStateMachine _levelStateMachine;
        private MusicRhythmManager _musicRhythmManager;
        private PlayerModel _playerModel;
        private SceneTransitionManager _sceneTransitionManager;
        private MonoAudioChannel _monoAudioChannel;

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            context
                .Resolve(out _gamePlayScene)
                .Resolve(out _levelStateMachine)
                .Resolve(out _sceneTransitionManager)
                .Resolve(out _autoPlayChartManager)
                .Resolve(out _musicRhythmManager)
                .Resolve(out _levelEntityManager)
                .Resolve(out _levelModel)
                .Resolve(out _endLevelPanel)
                .Resolve(out _monoAudioChannel)
                .Resolve(out _levelFeelManager);

            if (_gamePlayScene.TryGetPlayer(out _player))
            {
                _player.Context
                    .Resolve(out _playerModel);
            }
        }

        private void Start()
        {
            TypeEventSystem.Global.Register<EnemyDieEvent>(GlobalOnEnemyDie);
            TypeEventSystem.Global.Register<EnemyHurtEvent>(GlobalOnEnemyHurt);
            try
            {
                _levelStateMachine.OnWin += LevelWin;
                _levelStateMachine.OnFail += LevelFail;
                _autoPlayChartManager.OnRhythmHit += _levelEntityManager.OnRhythmHit;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Update()
        {
            DebugUpdate();
        }

        private void FixedUpdate()
        {
            if (!LevelRoot.Instance.IsStart)
            {
                return;
            }

            _levelModel.DangerScore -= DangerLevelConfigClass.NormalReductionPerSecond * Time.fixedDeltaTime;

            // check end
            if (_musicRhythmManager.IsMusicEnd)
            {
                _levelStateMachine.State = LevelStateMachine.LevelState.Win;
            }
        }

        private void OnDestroy()
        {
            TypeEventSystem.Global.UnRegister<EnemyDieEvent>(GlobalOnEnemyDie);
            TypeEventSystem.Global.UnRegister<EnemyHurtEvent>(GlobalOnEnemyHurt);
            _autoPlayChartManager.OnRhythmHit -= _levelEntityManager.OnRhythmHit;
            _levelStateMachine.OnWin -= LevelWin;
            _levelStateMachine.OnFail -= LevelFail;

            TimingCtrl.Instance.StopAllTimer();
        }

        private void GlobalOnEnemyHurt(EnemyHurtEvent para)
        {
            _levelFeelManager.CameraShake(token: _sceneTransitionManager.SceneChangedToken);
            //MonoAudioCtrl.Instance.PlayOneShot("Enemy_Hurt");
        }

        private void GlobalOnEnemyDie(EnemyDieEvent para)
        {
            //MonoAudioCtrl.Instance.PlayOneShot("Enemy_Die");
        }

        private void LevelFail()
        {
            _endLevelPanel.SetState(false);
            _endLevelPanel.Show();
            _monoAudioChannel.Play("Assets/Audio/GameFX/down.wav");

            UtilsCommands.SetTimer(3.0,
                () =>
                {
                    var monoGameRouter = MonoGameRouter.Instance;
                    monoGameRouter.LoadScene(monoGameRouter.CurrentScene);
                },
                _sceneTransitionManager.SceneChangedToken);
        }

        private void LevelWin()
        {
            _endLevelPanel.SetState(true);
            _endLevelPanel.Show();

            _musicRhythmManager.FadeOutAndStop(); // 当前音乐淡出 music fade out
            _monoAudioChannel.Play("Assets/Audio/GameFX/dingdong.wav");

            UtilsCommands.SetTimer(3.0,
                () =>
                {
                    var monoGameRouter = MonoGameRouter.Instance;
                    monoGameRouter.LoadScene(monoGameRouter.CurrentScene);
                },
                _sceneTransitionManager.SceneChangedToken);
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

            if (_player != null)
            {
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
        }
    }
}
