using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay;
using UnityEngine;
using UnityEngine.UI;

using THIS = Shuile.HUDHpBarElement;

namespace Shuile
{
    // TODO: maybe panel is not a good idea... esspecially Hide() and Show()
    public class HUDHpBarElement : BasePanelWithMono
    {
        private Image _image;

        public Image Image => _image;
        private void Awake()
            => this.RegisterUI<HUDHpBarElement>();
        private void OnDestroy()
            => this.UnRegisterUI<HUDHpBarElement>();

        public static PanelCreateor Creator = () =>
                Resources.Load<GameObject>("UIDesign/HUDHpBarElement").Instantiate().GetComponent<IPanel>();

        private void Start()
        {
            this.SetParent(UICtrl.Instance.WorldCanvas.transform);

            _image = gameObject.GetComponent<Image>();

        }

        public THIS Link(Enemy character)
        {
            // 监听血量变化
            character.OnHpChangedEvent += val =>
            {
                this.Image.fillAmount = (float)val / character.Property.healthPoint;
            };
            // 跟随玩家
            character.gameObject.AddComponent<UpdateEventMono>().OnUpdate += () =>
            {
                this.transform.position = character.transform.position + new Vector3(0, 1.5f); //Vector3(0, 1.5f): player height
            };

            return this;
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
