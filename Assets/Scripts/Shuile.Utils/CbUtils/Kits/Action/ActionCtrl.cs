using System.Collections;
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
        public DelayAction OnComplete(System.Action action)
        {
            onComplete += action;
            return this;
        }
        public void Start()
        {
            MonoActionCtrlExecutor.Instance.ExecuteCoroutine(DelayCoroutine());
        }

        private IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(delayDuration);
            onComplete?.Invoke();
        }
    }

    /*public class Sequence
    {
        public void Append()
        {

        }
        public void AppendInterval()
        {

        }
    }*/

    /// <summary>
    /// control timing sequence logic
    /// </summary>
    public class ActionCtrl: CSharpLazySingletons<ActionCtrl>
    {
        public DelayAction Delay(float durationInSeconds)
        {
            var delay = new DelayAction
            {
                delayDuration = durationInSeconds
            };
            return delay;
        }
    }
}
