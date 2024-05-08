using DG.Tweening;
using System.Collections;

using DelayTween = DG.Tweening.Core.TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions>;

namespace CbUtils
{
    /* [WIP]
     * last update: 2024.05.08/20:24
     */

    public class CoroutineActionCtrlExecutor : MonoSingletons<CoroutineActionCtrlExecutor>
    {
        protected override void OnAwake()
            => SetDontDestroyOnLoad();

        public void ExecuteCoroutine(IEnumerator coroutine)
            => StartCoroutine(coroutine);
    }

    public struct DotweenActionContainer
    {
        public DelayTween delayAction { get; set; }
        public float delayDuration { get; set; }
        public System.Action onComplete { get; set; }
    }
    public class DotweenActionCtrlExecutor : CSharpLazySingletons<DotweenActionCtrlExecutor>
    {
        public void Execute(DotweenActionContainer action)
        {
            DOTween.To(() => 0, x => { }, 1, action.delayDuration)
                .OnComplete(() => action.onComplete?.Invoke());
        }
    }
}
