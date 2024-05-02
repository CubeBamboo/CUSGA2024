using CbUtils;

using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Shuile
{
    public class MainMenuUIStateMachine : MonoBehaviour
    {
        [Header("Title")]
        [SerializeField] private RectTransform title;
        [SerializeField] private TextMeshProUGUI tips;

        private readonly Vector2 titleGameStartPos = new Vector2(0f, 640f);
        private readonly Vector2 titleInTitlePos = new Vector2(0f, 16f);
        private readonly Vector2 titleInMenuPos = new Vector2(0f, 170f);

        [Header("Menu")]
        [SerializeField] private RectTransform menuPanel;

        private readonly float MenuInitPosY = -740f;
        private readonly float MenuEnterPosY = -218f;

        //[Header("Select")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private RectTransform selectPanel;
        [SerializeField] private GameObject settingPanel;


        public enum State
        {
            Title,
            Menu,
            Select
        }

        private FSM<State> fsm;
        private void Awake()
        {
            InitUIState();
            InitFSM();
        }

        private void InitUIState()
        {
            startPanel.SetActive(true);

            tips.gameObject.SetActive(true);
            tips.alpha = 0;

            title.gameObject.SetActive(true);
            title.anchoredPosition = titleGameStartPos;

            menuPanel.gameObject.SetActive(true);
            menuPanel.anchoredPosition3D = menuPanel.anchoredPosition3D.With(y: MenuInitPosY);

            selectPanel.gameObject.SetActive(true);
            selectPanel.anchoredPosition3D = selectPanel.anchoredPosition3D.With(y: MenuInitPosY);

            settingPanel.SetActive(false);
        }

        private void InitFSM()
        {
            fsm = new();
            fsm.NewEventState(State.Title)
               .OnEnter(() =>
               {
                   title.DOAnchorPos(titleInTitlePos, 0.3f).SetEase(Ease.InOutSine);
                   tips.color = tips.color.With(a: 0);
                   tips.DOFade(0.4f, 0.8f)
                       .SetLoops(-1, LoopType.Yoyo)
                       .SetEase(Ease.InOutSine);
               })
               .OnExit(() =>
               {
                   title.DOAnchorPos(titleInMenuPos, 0.3f).SetEase(Ease.InOutSine);
                   tips.DOKill();
                   tips.DOFade(0, 0.3f);
               });
            fsm.NewEventState(State.Menu)
               .OnEnter(() =>
               {
                   menuPanel.DOAnchorPos3DY(MenuEnterPosY, 0.3f).SetEase(Ease.InOutSine);
               })
               .OnExit(() =>
               {
                   menuPanel.DOAnchorPos3DY(MenuInitPosY, 0.3f).SetEase(Ease.InOutSine);
               });
            fsm.NewEventState(State.Select)
               .OnEnter(() =>
               {
                   selectPanel.DOAnchorPos3DY(MenuEnterPosY, 0.3f).SetEase(Ease.InOutSine);
               })
               .OnExit(() =>
               {
                   selectPanel.DOAnchorPos3DY(MenuInitPosY, 0.3f).SetEase(Ease.InOutSine);
               });

            fsm.StartState(State.Title);
        }

        public void SwitchState(State state)
        {
            fsm.SwitchState(state);
        }

        public State CurrentState => fsm.CurrentStateId;
    }
}
