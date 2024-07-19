using UnityEngine;

namespace Shuile.UI.Gameplay
{
    public class EndLevelPanel : MonoBehaviour
    {
        [SerializeField] private GameObject win, fail;

        public void SetState(bool isWin)
        {
            win.SetActive(false);
            fail.SetActive(false);

            if (isWin)
            {
                win.SetActive(true);
            }
            else
            {
                fail.SetActive(true);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
