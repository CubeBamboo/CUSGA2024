using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using DelayTween = DG.Tweening.Core.TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions>;

namespace CbUtils.ActionKit
{
    public class OldDelayAction : IAction
    {
        private static readonly Dictionary<object, DelayTween> debounceCollection = new();

        public float delayDuration;

        private DelayTween delayTween;

        private object labelObject;
        public Action onComplete;

        public void Start()
        {
            Start(null);
        }

        public OldDelayAction OnComplete(Action action)
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

            if (labelObject != null)
            {
                debounceCollection.TryAdd(labelObject, delayTween);
            }

            if (gameObject != null)
            {
                SetLink(gameObject);
            }
        }

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
                if (labelObject != null)
                {
                    debounceCollection.Remove(labelObject);
                }

                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"DelayAction capture Exception: {e}");
            }
        }
    }
}
