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
        {
            this.RegisterUI<EndLevelPanel>();
            this.SetParent(UICtrl.Instance.OverlayCanvas.transform);
        }

        private void OnDestroy()
            => this.UnRegisterUI<EndLevelPanel>();

        private void Start()
        {
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
