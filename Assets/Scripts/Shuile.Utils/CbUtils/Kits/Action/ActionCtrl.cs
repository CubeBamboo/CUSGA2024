using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils.ActionKit
{
    public interface IAction
    {
        void Start();
    }

    public class DelayAction : IAction
    {
        public float delayDuration;
        public System.Action onComplete;

        private TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions> delayTween;

        public DelayAction OnComplete(System.Action action)
        {
            onComplete += action;
            return this;
        }

        /// <summary> life time will be linked to gameObject </summary>
        public void Start(GameObject gameObject)
        {
            delayTween = DOTween.To(() => 0, x => { }, 1, delayDuration)
                .OnComplete(HandleOnComplete);
            if(gameObject != null)
                SetLink(gameObject);
        }
        public void Start()
        {
            this.Start(null);
        }
        public void SetLink(GameObject gameObject)
        {
            delayTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
        public void Kill()
        {
            delayTween.Kill();
        }
        public void HandleOnComplete()
        {
            try
            {
                onComplete?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"DelayAction capture Exception: {e}");
            }
        }
    }

    public class NormalAction : IAction
    {
        public System.Action action;

        public NormalAction(System.Action action)
        {
            this.action = action;
        }

        public void Start()
        {
            action?.Invoke();
        }
    }

    /*public class Sequence : IAction
    {
        private DG.Tweening.Sequence sequence;
        private List<IAction> actions = new List<IAction>();

        public void AppendCallback(System.Action action)
        {
            sequence.AppendCallback(() => action());
        }
        public void AppendInterval(float seconds)
        {
            sequence.AppendInterval(seconds);
        }

        public void Start(GameObject gameObject)
        {
            sequence = DOTween.Sequence();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }*/

    /// <summary>
    /// control timing sequence logic
    /// </summary>
    public class ActionCtrl: CSharpLazySingletons<ActionCtrl>
    {
        public static DelayAction Delay(float durationInSeconds)
        {
            var delay = new DelayAction
            {
                delayDuration = durationInSeconds
            };
            return delay;
        }
    }
}
