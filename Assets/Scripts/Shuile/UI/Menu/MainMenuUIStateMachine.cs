using CbUtils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Menu
{
    public class MainMenuUIStateMachine : MonoBehaviour
    {
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

        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject selectPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Animation flashAnimation;
        [SerializeField] private Button btn_Start;
        [SerializeField] private Button btn_Setting;
        [SerializeField] private Button btn_Back;

        private FSM<State> _fsm;

        // [start panel]
        private RectTransform titleLogo;

        private AnimationState FlashAnimState => flashAnimation[flashAnimation.clip.name];

        public State CurrentState => _fsm.CurrentStateId;

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
            _fsm = new FSM<State>();
            _fsm.NewEventState(State.Menu)
                .OnEnter(() =>
                {
                    ((RectTransform)titleLogo.transform).DOAnchorPosY(106, 0.4f).SetEase(Ease.OutSine);
                    ((RectTransform)flashAnimation.transform).DOAnchorPos(new Vector2(-215f, 56f), 0.3f)
                        .SetEase(Ease.OutSine);
                    RewindFlash();
                })
                .OnExit(() =>
                {
                    ((RectTransform)titleLogo.transform).DOAnchorPosY(1000, 0.4f).SetEase(Ease.OutSine);
                });
            _fsm.NewEventState(State.Select)
                .OnEnter(() =>
                {
                    PlayFlash();
                    btn_Back.transform.parent.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f).SetEase(Ease.OutSine);
                    btn_Setting.targetGraphic.DOFade(0f, 0.3f);
                    btn_Setting.enabled = false;
                    btn_Start.targetGraphic.DOFade(0f, 0.3f);
                    btn_Start.enabled = false;
                    selectPanel.SetActive(true);
                    selectPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
                    settingsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                })
                .OnExit(() =>
                {
                    btn_Back.transform.parent.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f).SetEase(Ease.OutSine);
                    btn_Setting.targetGraphic.DOFade(1f, 0.3f);
                    btn_Setting.enabled = true;
                    btn_Start.targetGraphic.DOFade(1f, 0.3f);
                    btn_Start.enabled = true;
                    selectPanel.SetActive(false);
                    selectPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.3f);
                    settingsPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
                });
            _fsm.NewEventState(State.Settings)
                .OnEnter(() =>
                {
                    btn_Setting.enabled = false;
                    PlayFlash();
                    ((RectTransform)flashAnimation.transform).DOAnchorPos(new Vector2(-330, 350), 0.3f)
                        .SetEase(Ease.OutSine);
                    btn_Back.transform.parent.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f).SetEase(Ease.OutSine);
                    btn_Setting.transform.DORotate(new Vector3(0f, 0f, 0), 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)btn_Start.transform).DOAnchorPosY(300f, 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)btn_Setting.transform).DOAnchorPosY(-215f, 0.3f).SetEase(Ease.OutSine);
                    ((RectTransform)settingsPanel.transform).DOAnchorPosY(-325, 0.1f).SetEase(Ease.OutSine);
                    settingsPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
                })
                .OnExit(() =>
                {
                    btn_Setting.enabled = true;
                    btn_Back.transform.parent.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f).SetEase(Ease.OutSine);
                    btn_Setting.transform.DORotate(new Vector3(0f, 0f, 18.4f), 0.3f).SetEase(Ease.OutSine);
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
    }
}
