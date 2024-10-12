using UnityEngine;

namespace Shuile.UI.Gameplay
{
    public class GameplayExitPanel : MonoBehaviour
    {
        [SerializeField] private Animator exitBarAnimator;

        private void LateUpdate()
        {
            if (exitBarAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f)
            {
                exitBarAnimator.SetFloat("Speed", 0f);
            }
        }

        public void Hide()
        {
            exitBarAnimator.SetFloat("Speed", -1f);
        }

        public void Show()
        {
            exitBarAnimator.SetFloat("Speed", 1f);
        }

        public void Exit()
        {
            MonoGameRouter.Instance.LoadFromName("MainMenu");
        }
    }
}
