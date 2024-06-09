using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Menu
{
    public class MainMenuPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_Start;
        [SerializeField] private Button btn_Settings;
        [SerializeField] private Button btn_Help;
        [SerializeField] private Button btn_Exit;
        [SerializeField] private Button helpPage;

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
            btn_Settings.onClick.AddListener(() =>
            {
                mainMenu.SwitchState(MainMenuUIStateMachine.State.Settings);
            });
            btn_Help.onClick.AddListener(() =>
            {
                helpPage.gameObject.SetActive(true);
            });
            helpPage.onClick.AddListener(() =>
            {
                helpPage.gameObject.SetActive(false);
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
