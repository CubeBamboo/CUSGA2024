using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shuile.UI
{
    public class EndStaticsButton : Button
    {
        private Vector3 _originScale;
        private BindableProperty<State> _currentState;

        private bool _isInBound;

        protected override void Awake()
        {
            base.Awake();
            _originScale = transform.localScale;
            transition = Transition.None;

            _currentState = new BindableProperty<State>();
            _currentState.BindValueChanged((_, state) =>
            {
                switch (state)
                {
                    case State.Idle:
                        transform.DOKill();
                        transform.DOScale(_originScale, 0.4f).SetEase(Ease.OutCubic);
                        break;
                    case State.Hover:
                        transform.DOKill();
                        transform.DOScale(_originScale * 1.1f, 0.4f).SetEase(Ease.OutCubic);
                        break;
                    case State.ClickOn:
                        transform.DOKill();
                        transform.DOScale(_originScale * 0.9f, 0.15f).SetEase(Ease.OutCubic);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            });
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _isInBound = true;
            _currentState.Value = State.Hover;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _currentState.Value = State.ClickOn;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _currentState.Value = _isInBound ? State.Hover : State.Idle;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isInBound = false;
            _currentState.Value = State.Idle;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            transform.DOKill();
        }

        private enum State
        {
            Idle, Hover, ClickOn
        }
    }
}
