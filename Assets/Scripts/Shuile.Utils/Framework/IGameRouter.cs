using CbUtils.ActionKit;
using CbUtils.Timing;
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

            var exitTimer = TimingCtrl.Instance.Timer(loadingViewer.ExitDuration,
                () => loadingViewer.OnEnd());
            var enterTimer = TimingCtrl.Instance.Timer(loadingViewer.InDuration, () =>
            {
                action?.Invoke();
                loadingViewer.Out();
                exitTimer.Start();
            });
            enterTimer.Start();
            // 没有使用序列导致的抽象写法 not using sequence leads to such a fuuuuuuckkking code :((((
        }

        public static void DoTransition(this IGameRouter gameRouter,
            System.Func<UniTask> asyncAction, IRouterLoadingViewer loadingViewer)
        {
            loadingViewer.OnStart();
            loadingViewer.In();

            var exitTimer = TimingCtrl.Instance.Timer(loadingViewer.ExitDuration,
                () => loadingViewer.OnEnd());
            var enterTimer = TimingCtrl.Instance.Timer(loadingViewer.InDuration, async () =>
            {
                if (asyncAction != null)
                    await asyncAction.Invoke();
                loadingViewer.Out();
                exitTimer.Start();
            });
            enterTimer.Start();
            // 没有使用序列导致的抽象写法 not using sequence leads to such a fuuuuuuckkking code :((((
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
