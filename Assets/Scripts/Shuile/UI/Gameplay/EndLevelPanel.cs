using Shuile.Framework;
using CbUtils.Extension;

using UnityEngine;
using TMPro;

namespace Shuile
{
    public class EndLevelPanel : BasePanelWithMono
    {
        [SerializeField] private GameObject win, fail;

        public static PanelCreateor Creator = () =>
            Resources.Load<GameObject>("UIDesign/EndLevelPanel").Instantiate().GetComponent<IPanel>();

        private void Awake()
        {
            this.RegisterUI<EndLevelPanel>();
            this.SetParent(UICtrl.Instance.OverlayCanvas.transform);
        }

        private void OnDestroy()
            => this.UnRegisterUI<EndLevelPanel>();

        public void SetState(bool isWin)
        {
            win.SetActive(false);
            fail.SetActive(false);

            if(isWin) win.SetActive(true);
            else fail.SetActive(true);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
