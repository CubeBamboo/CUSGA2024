using CbUtils.ActionKit;
using Cysharp.Threading.Tasks;

namespace Shuile
{
    public interface IGameRouter { }
    public static class IGameRouterExt
    {
        public static void DoTransition(this IGameRouter gameRouter,
            System.Action action, IRouterLoadingViewer loadingViewer)
        {
            loadingViewer.OnStart();
            loadingViewer.In();

            var exitDelay = ActionCtrl.Instance.Delay(loadingViewer.ExitDuration)
                .OnComplete(() => loadingViewer.OnEnd());
            var enterDelay = ActionCtrl.Instance.Delay(loadingViewer.InDuration)
                .OnComplete(() => { action?.Invoke(); loadingViewer.Out(); exitDelay.Start(); });
            enterDelay.Start(); // 没有使用序列导致的抽象写法 not using sequence leads to such a fuuuuuuckkking code :((((
        }

        public static void DoTransition(this IGameRouter gameRouter,
            System.Func<UniTask> asyncAction, IRouterLoadingViewer loadingViewer)
        {
            loadingViewer.OnStart();
            loadingViewer.In();

            var exitDelay = ActionCtrl.Instance.Delay(loadingViewer.ExitDuration)
                .OnComplete(() => loadingViewer.OnEnd());
            var enterDelay = ActionCtrl.Instance.Delay(loadingViewer.InDuration)
                .OnComplete(async () => {
                    if (asyncAction != null)
                        await asyncAction.Invoke();
                    loadingViewer.Out();
                    exitDelay.Start();
                });
            enterDelay.Start(); // 没有使用序列导致的抽象写法 not using sequence leads to such a fuuuuuuckkking code :((((
        }
    }

    public interface IRouterLoadingViewer
    {
        public float InDuration { get; }
        public float ExitDuration { get; }
        public void OnStart();
        public void In();
        public void Out();
        public void OnEnd();
    }
}
