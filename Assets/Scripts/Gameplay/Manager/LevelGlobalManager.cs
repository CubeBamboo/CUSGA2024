using CbUtils.Event;
using Shuile.Gameplay.Event;
using Shuile.Rhythm.Runtime;
using System;
using UnityEngine;

using UInput = UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    // if you dont know where to put the logic, put it here
    public class LevelGlobalManager : MonoBehaviour
    {
        private LevelModel levelModel;

        private void Awake()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();

            gameObject.AddComponent<UpdateEventMono>().OnUpdate += DebugUpdate;
        }

        private void Start()
        {
            EnemyDieEvent.Register(GlobalOnEnemyDie);
            EnemyHurtEvent.Register(GlobalOnEnemyHurt);
        }

        private void OnDestroy()
        {
            EnemyDieEvent.UnRegister(GlobalOnEnemyDie);
            EnemyHurtEvent.UnRegister(GlobalOnEnemyHurt);
        }
        private void FixedUpdate()
        {
            levelModel.DangerScore -= DangerLevelConfigClass.NormalReductionPerSecond * Time.fixedDeltaTime;

            // check end
            if (MusicRhythmManager.Instance.IsMusicEnd)
            {
                LevelStateMachine.Instance.State = LevelStateMachine.LevelState.Win;
            }
        }

        private void GlobalOnEnemyHurt(GameObject @object)
        {
            LevelFeelManager.Instance.CameraShake();
            MonoAudioCtrl.Instance.PlayOneShot("Enemy_Hurt");
        }
        private void GlobalOnEnemyDie(GameObject go)
        {
            MonoAudioCtrl.Instance.PlayOneShot("Enemy_Die");
        }

        private void DebugUpdate()
        {
            if (UInput.Keyboard.current.zKey.wasPressedThisFrame)
            {
                MusicRhythmManager.Instance.SetCurrentTime(MusicRhythmManager.Instance.CurrentTime + 5f);
            }
            if (UInput.Keyboard.current.xKey.wasPressedThisFrame)
            {
                MusicRhythmManager.Instance.SetCurrentTime(MusicRhythmManager.Instance.CurrentTime - 5f);
            }
        }
    }
}
