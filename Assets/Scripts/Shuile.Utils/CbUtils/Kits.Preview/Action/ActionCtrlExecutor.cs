using CbUtils.ActionKit;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using DelayTween = DG.Tweening.Core.TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions>;

namespace CbUtils
{
    /* [WIP]
     * last update: 2024.05.14/14:49
     */

    /* support:
     * - fixed delay
     * - complete callback
    */
    public class CoroutineActionCtrlExecutor : MonoSingletons<CoroutineActionCtrlExecutor>
    {
        protected override void OnAwake()
            => SetDontDestroyOnLoad();

        public void ExecuteCoroutine(IEnumerator coroutine)
            => StartCoroutine(coroutine);

        public void Execute(DelayAction.DelayActionData data)
        {
            StartCoroutine(InternalExecuteCoroutine(data));
        }

        private IEnumerator InternalExecuteCoroutine(DelayAction.DelayActionData data)
        {
            yield return new WaitForSeconds(data.delayDuration);
            data.onComplete?.Invoke();
        }
    }

    /* support:
     * - fixed delay
     * - complete callback
     * - safe trigger (callback with try-catch)
     * - linked Gameobject
     */
    public class DotweenActionCtrlExecutor : CSharpLazySingletons<DotweenActionCtrlExecutor>
    {
        public void Execute(DelayAction.DelayActionData data, bool safeTrigger = true)
        {
            System.Action completeAction = safeTrigger ? SafeTriggerWarpper(data.onComplete) : data.onComplete;
            DOTween.To(() => 0, x => { }, 1, data.delayDuration)
                .OnComplete(() => completeAction?.Invoke())
                .SetLink(data.linkedGameobject, LinkBehaviour.KillOnDestroy);
        }

        private System.Action SafeTriggerWarpper(System.Action origin)
        {
            return () =>
            {
                try
                {
                    origin?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"DelayAction capture Exception: {e}");
                }
            };
        }
    }
}
