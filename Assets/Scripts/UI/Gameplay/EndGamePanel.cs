using CbUtils;
using Shuile.Framework;

using UnityEngine;
using TMPro;

namespace Shuile
{
    public class EndGamePanel : BasePanelInUnity
    {
        private GameObject panel;
        private TextMeshProUGUI timeTextUGUI;

        public TextMeshProUGUI TimeTextUGUI => timeTextUGUI;
        public override void BeforeInit()
        {
            var panelAssets = Resources.Load<GameObject>("UIDesign/GameEndPanel");
            panel = panelAssets.Instantiate();
            timeTextUGUI = panel.transform.Find("Text_Time").GetComponent<TextMeshProUGUI>();
        }

        protected override GameObject Panel => panel;
        protected override Canvas Canvas => UICtrl.Instance.OverlayCanvas;

        public override void DeInit()
        {
            panel.Destroy();
        }

        public override void Show()
        {
            panel.SetActive(true);
        }

        public override void Hide()
        {
            panel.SetActive(false);
        }
    }
}
