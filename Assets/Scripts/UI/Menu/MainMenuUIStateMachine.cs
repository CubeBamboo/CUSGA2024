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

        [Header("Select")]
        [SerializeField] private RectTransform selectLevelParent;
        [SerializeField] private RectTransform exitButton;


        public enum State
        {
            Title,
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
            title.anchoredPosition3D = new Vector3(0, 1000, 0);
            tips.alpha = 0;

            selectLevelParent.anchoredPosition3D = new Vector3(0, -800, 0);
            exitButton.anchoredPosition3D = new Vector3(773, -1000, 0);
        }

        private void InitFSM()
        {
            fsm = new();
            fsm.NewEventState(State.Title)
               .OnEnter(() =>
               {
                   title.DOAnchorPos3DY(16, 0.6f);
                   tips.color = tips.color.With(a: 0);
                   tips.DOFade(0.4f, 0.8f)
                       .SetLoops(-1, LoopType.Yoyo)
                       .SetEase(Ease.InOutSine);
               })
               .OnExit(() =>
               {
                   title.DOAnchorPos3DY(1000, 0.6f);
                   tips.DOKill();
                   tips.DOFade(0, 0.3f);
               });
            fsm.NewEventState(State.Select)
               .OnEnter(() =>
               {
                   selectLevelParent.DOAnchorPos3DY(0, 0.6f);
                   exitButton.DOAnchorPos3DY(-400, 0.6f);
               })
               .OnExit(() =>
               {
                   selectLevelParent.DOAnchorPos3DY(-800, 0.6f);
                   exitButton.DOAnchorPos3DY(-1000, 0.6f);
               });

            fsm.StartState(State.Title);
        }

        public void SwitchState(State state)
        {
            fsm.SwitchState(state);
        }
    }
}
