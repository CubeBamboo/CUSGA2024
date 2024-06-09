using CbUtils.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI
{
    public class GlobalTransitionViewer : MonoSingletons<GlobalTransitionViewer>, IRouterLoadingViewer
    {
        [SerializeField] private Image panel;
        public float inDuration = 0.4f;
        public float outDuration = 0.4f;

        public float InDuration => inDuration;
        public float ExitDuration => outDuration;

        protected override void OnAwake()
        {
            panel.color = panel.color.With(a: 0);
        }

        public void OnStart()
        {
            gameObject.SetActive(true);
        }

        public void In()
        {
            panel.DOFade(1, inDuration);
        }

        public void Out()
        {
            panel.DOFade(0, inDuration);
        }

        public void OnEnd()
        {
            gameObject.SetActive(false);
        }
    }
}
