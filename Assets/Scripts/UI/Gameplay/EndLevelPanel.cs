using Shuile.Framework;
using CbUtils;

using UnityEngine;
using TMPro;

namespace Shuile
{
    public class EndLevelPanel : BasePanelWithMono
    {
        [SerializeField] private TextMeshProUGUI timeTextUGUI;
        public TextMeshProUGUI TimeTextUGUI => timeTextUGUI;

        public static PanelCreateor Creator = () =>
            Resources.Load<GameObject>("UIDesign/EndLevelPanel").Instantiate().GetComponent<IPanel>();

        private void Awake()
            => this.RegisterUI<EndLevelPanel>();

        private void OnDestroy()
            => this.UnRegisterUI<EndLevelPanel>();

        private void Start()
        {
            this.SetParent(UICtrl.Instance.OverlayCanvas.transform);
            Hide();
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
