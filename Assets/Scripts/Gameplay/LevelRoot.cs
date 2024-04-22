using CbUtils;
using Shuile.Framework;
using Shuile.UI;
using Shuile.Rhythm.Runtime;
using CbUtils.Event;

using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UDebug = UnityEngine.Debug;
using Shuile.Gameplay;

namespace Shuile.Root
{
    /* link to all objects which want to depend on unity level scene (like lifeTime control)
     * Contain:
     * whole GameplayService
     * UICtrl's GameplayPart
     */
    //it also contains the level state callback
    public class LevelRoot : MonoNonAutoSpawnSingletons<LevelRoot>
    {
        public bool needHitWithRhythm { get; private set; }

        protected override void OnAwake()
        {
            GameplayService.Interface.OnInit();
            LevelDataBinder.Instance.Initialize();

            UICtrl.Instance.RegisterCreator<EndLevelPanel>(EndLevelPanel.Creator);
            UICtrl.Instance.RegisterCreator<HUDHpBarElement>(HUDHpBarElement.Creator);
            needHitWithRhythm = LevelResources.Instance.debugSettings.needHitWithRhythm;
        }
        private void OnDestroy()
        {
            UICtrl.Instance.UnRegisterCreator<EndLevelPanel>();
            UICtrl.Instance.UnRegisterCreator<HUDHpBarElement>();

            LevelDataBinder.Instance.DeInitialize();
            GameplayService.Interface.OnDeInit();
        }
        private void Start()
        {
            UICtrl.Instance.Create<EndLevelPanel>().Hide();
            UICtrl.Instance.Get<PlayingPanel>().Show();
            UICtrl.Instance.Get<DebugPanel>().Show();

            this.gameObject.AddComponent<UpdateEventMono>().OnUpdate += () =>
            {
                // TODO: [!] for debug
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    SceneManager.LoadScene("MainMenu");
                }
            };
        }
    }
}
