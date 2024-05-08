using CbUtils;

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    public class MainMenuUIStateMachine : MonoBehaviour
    {
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject selectPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Animation flashAnimation;
        [SerializeField] private Button btn_Start;
        [SerializeField] private Button btn_Setting;
        [SerializeField] private Button btn_Back;

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

        private FSM<State> _fsm;
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
            _fsm = new();
            _fsm.NewEventState(State.Menu)
                .OnEnter(() =>
                {
                    ((RectTransform)titleLogo.transform).DOAnchorPosY(106, 0.4f).SetEase(Ease.OutSine);
                    ((RectTransform)flashAnimation.transform).DOAnchorPos(new(-215f, 56f), 0.3f).SetEase(Ease.OutSine);
                    RewindFlash();
                })
                .OnExit(() =>
                {
                    ((RectTransform)titleLogo.transform).DOAnchorPosY(1000, 0.4f).SetEase(Ease.OutSine);
                });
            _fsm.NewEventState(State.Select)
                .OnEnter(() =>
                {
                    selectPanel.SetActive(true);
                    PlayFlash();
                })
                .OnExit(() =>
                {
                    selectPanel.SetActive(false);
                });
            _fsm.NewEventState(State.Settings)
                .OnEnter(() =>
                {
                    btn_Setting.enabled = false;
                    PlayFlash();
                    ((RectTransform)flashAnimation.transform).DOAnchorPos(new(-330, 350), 0.3f).SetEase(Ease.OutSine);
                    btn_Back.transform.parent.DOLocalRotate(new(0f, 0f, 20f), 0.2f).SetEase(Ease.OutSine);
                    btn_Setting.transform.DORotate(new(0f, 0f, 0), 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)btn_Start.transform).DOAnchorPosY(300f, 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)btn_Setting.transform).DOAnchorPosY(-215f, 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)settingsPanel.transform).DOAnchorPosY(-325, 0.1f).SetEase(Ease.OutSine);
                    settingsPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
                })
                .OnExit(() =>
                {
                    btn_Setting.enabled = true;
                    btn_Back.transform.parent.DOLocalRotate(new(0f, 0f, -20f), 0.2f).SetEase(Ease.OutSine);
                    btn_Setting.transform.DORotate(new(0f, 0f, 18.4f), 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)btn_Start.transform).DOAnchorPosY(-486f, 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)btn_Setting.transform).DOAnchorPosY(-710f, 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)settingsPanel.transform).DOAnchorPosY(-600, 0.1f).SetEase(Ease.OutSine);
                    settingsPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.1f);
                });

            _fsm.StartState(State.Menu);
        }

        public void SwitchState(State state)
        {
            _fsm.SwitchState(state);
        }

        private void PlayFlash()
        {
            FlashAnimState.time = FlashAnimState.normalizedTime < 0f ? 0f : FlashAnimState.time;
            FlashAnimState.speed = 1f;
            flashAnimation.Play();
        }
        private void RewindFlash()
        {
            FlashAnimState.time = FlashAnimState.normalizedTime > 1f ? flashAnimation.clip.length : FlashAnimState.time;
            FlashAnimState.speed = -1f;
            flashAnimation.Play();
        }

        private AnimationState FlashAnimState => flashAnimation[flashAnimation.clip.name];

        public State CurrentState => _fsm.CurrentStateId;
    }
}
