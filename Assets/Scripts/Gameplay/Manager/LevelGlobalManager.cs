using CbUtils.Event;
using Shuile.Rhythm.Runtime;

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

            gameObject.AddComponent<UpdateEventMono>().OnUpdate += () =>
            {
                if (UInput.Keyboard.current.zKey.wasPressedThisFrame)
                {
                    MusicRhythmManager.Instance.SetCurrentTime(MusicRhythmManager.Instance.CurrentTime + 5f);
                }
                if (UInput.Keyboard.current.xKey.wasPressedThisFrame)
                {
                    MusicRhythmManager.Instance.SetCurrentTime(MusicRhythmManager.Instance.CurrentTime - 5f);
                }
            };
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
    }
}
