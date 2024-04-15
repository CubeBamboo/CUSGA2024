using System.Collections;
using UnityEngine;

namespace CbUtils
{
    public interface IAction
    {
        void Start();
    }

    public class DelayAction
    {
        public float delayDuration;
        public System.Action onComplete;
        public DelayAction OnComplete(System.Action action)
        {
            onComplete += action;
            return this;
        }
        public void Start()
        {
            ActionCtrl.Instance.StartCoroutine(DelayCoroutine());
        }

        private IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(delayDuration);
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// control timing sequence logic
    /// </summary>
    public class ActionCtrl: MonoSingletons<ActionCtrl>
    {
        public DelayAction Delay(float duration)
        {
            var delay = new DelayAction();
            delay.delayDuration = duration;
            return delay;
        }
    }
}
