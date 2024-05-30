using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

using DelayTween = DG.Tweening.Core.TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions>;

namespace CbUtils.ActionKit
{
    public class OldDelayAction : IAction
    {
        private static Dictionary<object, DelayTween> debounceCollection = new();

        public float delayDuration;
        public System.Action onComplete;

        private object labelObject;

        private DelayTween delayTween;

        public OldDelayAction OnComplete(System.Action action)
        {
            onComplete += action;
            return this;
        }

        /// <summary> life time will be linked to gameObject </summary>
        public void Start(GameObject gameObject)
        {
            if (labelObject != null && debounceCollection.ContainsKey(labelObject)) // debounce
            {
                debounceCollection[labelObject].Kill();
            }

            var delayTween = DOTween.To(() => 0, x => { }, 1, delayDuration)
                .OnComplete(() => onComplete?.Invoke());

            if(labelObject != null)
            {
                debounceCollection.TryAdd(labelObject, delayTween);
            }

            if (gameObject != null)
                SetLink(gameObject);
        }
        public void Start() => this.Start(null);

        public OldDelayAction SetDebounce(object label)
        {
            labelObject = label;
            return this;
        }

        public OldDelayAction SetLink(GameObject gameObject)
        {
            delayTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            return this;
        }
        public void Kill()
        {
            delayTween.Kill();
        }
        public void HandleOnComplete()
        {
            try
            {
                if(labelObject != null) debounceCollection.Remove(labelObject);
                onComplete?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"DelayAction capture Exception: {e}");
            }
        }
    }
}
