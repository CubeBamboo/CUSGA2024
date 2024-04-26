using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    public class MainMenuPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_Start;
        [SerializeField] private Button btn_Settings;
        [SerializeField] private Button btn_Credits;
        [SerializeField] private Button btn_Exit;

        [SerializeField] private MainMenuUIStateMachine mainMenu;

        private void Awake()
        {
            ConfigureButtonEvent();
        }

        private void ConfigureButtonEvent()
        {
            btn_Exit.onClick.AddListener(ExitGame);

            btn_Start.onClick.AddListener(() =>
            {
                mainMenu.SwitchState(MainMenuUIStateMachine.State.Select);
            });
        }

        public void ExitGame()
        {
#if !UNITY_EDITOR
            Application.Quit();
#else
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
