using UnityEngine;
using TMPro;

using CbUtils;
using Shuile.Gameplay;
using Shuile.Framework;


namespace Shuile.UI
{
    public class PlayingPanel : BasePanelInUnity
    {
        private GameObject _panel;
        private TextMeshProUGUI hpTextUGUI;

        public TextMeshProUGUI HpTextUGUI => hpTextUGUI;

        public override void BeforeInit()
        {
            var assets = Resources.Load<GameObject>("UIDesign/PlayingPanel");
            _panel = assets.Instantiate();
            hpTextUGUI = _panel.transform.Find("Text_Hp").GetComponent<TextMeshProUGUI>();

            GameplayService.Interface.Get<PlayerController>().OnHpChangedEvent += (val) =>
            {
                hpTextUGUI.text = "Player HP: " + (val > 0 ? val : 0).ToString();
            };
        }

        protected override GameObject Panel => _panel;
        protected override Canvas Canvas => UICtrl.Instance.OverlayCanvas;
        public override void DeInit()
        {
            _panel.Destroy();
        }

        public override void Hide()
        {
            _panel.SetActive(false);
        }

        public override void Show()
        {
            _panel.SetActive(true);
        }
    }
}
