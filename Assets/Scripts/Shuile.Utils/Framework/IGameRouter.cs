using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

namespace Shuile
{
    public interface IGameRouter
    {
    }

    public static class IGameRouterExt
    {
        public static void DoTransition(this IGameRouter gameRouter,
            Action action, IRouterLoadingViewer loadingViewer)
        {
            loadingViewer.OnStart();
            loadingViewer.In();

            DOTween.Sequence()
                .AppendInterval(loadingViewer.InDuration)
                .AppendCallback(() =>
                {
                    action?.Invoke();
                    loadingViewer.Out();
                })
                .AppendInterval(loadingViewer.ExitDuration)
                .AppendCallback(loadingViewer.OnEnd);
        }

        public static void DoTransition(this IGameRouter gameRouter,
            Func<UniTask> asyncAction, IRouterLoadingViewer loadingViewer)
        {
            loadingViewer.OnStart();
            loadingViewer.In();

            DOTween.Sequence()
                .AppendInterval(loadingViewer.InDuration)
                .AppendCallback(async () =>
                {
                    if (asyncAction != null)
                    {
                        await asyncAction.Invoke();
                    }

                    loadingViewer.Out();
                })
                .AppendInterval(loadingViewer.ExitDuration)
                .AppendCallback(loadingViewer.OnEnd);
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
