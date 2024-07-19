using System;
using UnityEngine;

namespace Shuile
{
    public abstract class MonoScriptAnimation : MonoBehaviour
    {
        public bool PlayOnAwake = true;
        public bool Loop;

        private bool _completed;

        public bool IsCompleted
        {
            get => _completed;
            protected set
            {
                _completed = value;
                if (_completed)
                {
                    OnComplete?.Invoke();
                }
            }
        }

        protected void Awake()
        {
            if (Loop)
            {
                OnComplete += () =>
                {
                    StopAnimation();
                    Play();
                };
            }

            OnAwake();
        }

        protected void Start()
        {
            if (PlayOnAwake)
            {
                Play();
            }
        }

        protected void OnDestroy()
        {
            StopAnimation();
            OnComplete = null;
        }

        public event Action OnComplete;

        protected virtual void OnAwake() { }

        /// <summary> set <seealso cref="IsCompleted" /> to true to specify when the animation is completed </summary>
        protected abstract void OnPlayAnimation();

        protected abstract void StopAnimation();

        public void Play()
        {
            OnPlayAnimation();
        }

        public void Stop()
        {
            StopAnimation();
        }
    }

    // example
    /*protected override void OnPlayAnimation() // sync animation
    {
        _title.rectTransform.DOShakeAnchorPos(5f, 0.01f);
        DOTween.Sequence()
               .Append(_button1.DOAnchorPos3DY(1080, 0.5f).From())
               .AppendInterval(0.5f)
               .Append(_button2.DOAnchorPos3DY(1080, 0.5f).From())
               .AppendInterval(0.5f)
               .Append(_button3.DOAnchorPos3DY(1080, 0.5f).From())
               .OnComplete(() => IsCompleted = true); // call IsCompleted
    }*/
}
