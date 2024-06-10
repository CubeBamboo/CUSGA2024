using Shuile.Framework;
using UnityEngine;

namespace Shuile.UI.Gameplay
{
    public class GameplayExitPanel : MonoBehaviour
    {
        [SerializeField] private Animator exitBarAnimator;

        public void Hide()
        {
            exitBarAnimator.SetFloat("Speed", -1f);
        }

        public void Show()
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