using Shuile.Framework;

using UnityEngine;

namespace Shuile.UI
{
    public class GameplayExitPanel : BasePanelWithMono
    {
        [SerializeField] private Animator exitBarAnimator;

        private void Awake()
            => this.RegisterUI();
        private void OnDestroy()
            => this.UnRegisterUI();

        public override void Hide()
        {
            exitBarAnimator.SetFloat("Speed", -1f);
        }

        public override void Show()
        {
            exitBarAnimator.SetFloat("Speed", 1f);
        }

        private void LateUpdate()
        {
            if (exitBarAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f)
                exitBarAnimator.SetFloat("Speed", 0f);
        }

        public void Exit()
        {
            MonoGameRouter.Instance.ToMenu();
        }
    }
}