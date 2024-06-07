using CbUtils.Extension;
using CbUtils.Kits.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    public class TaskBusyScreen : MonoBehaviour, IBusyScreen
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Animation animComponent;

        private void Awake()
        {
            canvasGroup.alpha = 0;
            animComponent.Stop();
        }

        public void Hide()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0.0f, 0.5f).OnComplete(() => animComponent.Stop());
        }

        public void Show()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0.75f, 0.5f);
            animComponent.Play();
        }

        public void OnInterrupt()
        {
            canvasGroup.DOKill();
        }
    }
}
