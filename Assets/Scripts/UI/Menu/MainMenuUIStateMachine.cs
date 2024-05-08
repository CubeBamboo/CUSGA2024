using CbUtils;

using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Shuile
{
    public class MainMenuUIStateMachine : MonoBehaviour
    {
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject selectPanel;
        [SerializeField] private GameObject settingsPanel;

        // [start panel]
        private RectTransform titleLogo;

        //private readonly Vector2 titleLogoStartPos = new Vector2(0f, 640f);
        //private readonly Vector2 titleInTitlePos = new Vector2(0f, 16f);
        //private readonly Vector2 titleInMenuPos = new Vector2(0f, 170f);

        //[Header("Menu")]
        //[SerializeField] private RectTransform menuPanel;

        //private readonly float MenuInitPosY = -740f;
        //private readonly float MenuEnterPosY = -218f;

        //[Header("Select")]
        //[SerializeField] private GameObject settingPanel;

        public enum State
        {
            Menu,
            Select,
            Settings
        }

        private FSM<State> fsm;
        private void Awake()
        {
            InitUIComponent();
            InitUIState();
            InitFSM();
        }

        private void InitUIComponent()
        {
            titleLogo = titlePanel.transform.Find("Title").GetComponent<RectTransform>();
        }

        private void InitUIState()
        {
            titlePanel.SetActive(true);
            selectPanel.SetActive(false);
            settingsPanel.SetActive(false);

            //tips.gameObject.SetActive(true);
            //tips.alpha = 0;

            //title.gameObject.SetActive(true);
            //title.anchoredPosition = titleGameStartPos;

            //menuPanel.gameObject.SetActive(true);
            //menuPanel.anchoredPosition3D = menuPanel.anchoredPosition3D.With(y: MenuInitPosY);

            //selectPanel.gameObject.SetActive(true);
            //selectPanel.anchoredPosition3D = selectPanel.anchoredPosition3D.With(y: MenuInitPosY);

            //settingPanel.SetActive(false);
        }

        private void InitFSM()
        {
            fsm = new();
            fsm.NewEventState(State.Menu)
               .OnEnter(() =>
               {
                   //menuPanel.DOAnchorPos3DY(MenuEnterPosY, 0.3f).SetEase(Ease.InOutSine);
                   titlePanel.SetActive(true);
               })
               .OnExit(() =>
               {
                   //menuPanel.DOAnchorPos3DY(MenuInitPosY, 0.3f).SetEase(Ease.InOutSine);
                   titlePanel.SetActive(false);
               });
            fsm.NewEventState(State.Select)
               .OnEnter(() =>
               {
                   //selectPanel.DOAnchorPos3DY(MenuEnterPosY, 0.3f).SetEase(Ease.InOutSine);
                   selectPanel.SetActive(true);
               })
               .OnExit(() =>
               {
                   //selectPanel.DOAnchorPos3DY(MenuInitPosY, 0.3f).SetEase(Ease.InOutSine);
                   selectPanel.SetActive(false);
               });
            fsm.NewEventState(State.Settings)
               .OnEnter(() =>
               {
                   settingsPanel.SetActive(true);
               })
               .OnExit(() =>
               {
                   settingsPanel.SetActive(false);
               });

            fsm.StartState(State.Menu);
        }

        public void SwitchState(State state)
        {
            fsm.SwitchState(state);
        }

        public State CurrentState => fsm.CurrentStateId;
    }
}
