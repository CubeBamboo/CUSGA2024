using CbUtils.Extension;
using CbUtils.Preview.Event;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Entity;
using UnityEngine;
using UnityEngine.UI;
using THIS = Shuile.UI.Gameplay.HUDHpBarElement;

namespace Shuile.UI.Gameplay
{
    // TODO: maybe panel is not a good idea... esspecially Hide() and Show()
    public class HUDHpBarElement : BasePanelWithMono
    {
        public Image Image { get; private set; }

        private void Awake()
            => this.RegisterUI<HUDHpBarElement>();
        private void OnDestroy()
            => this.UnRegisterUI<HUDHpBarElement>();

        public static readonly PanelCreator Creator = () =>
                Resources.Load<GameObject>("UIDesign/HUDHpBarElement").Instantiate().GetComponent<IPanel>();

        private void Start()
        {
            this.SetParent(UICtrl.Instance.WorldCanvas.transform);

            Image = gameObject.GetComponent<Image>();

        }

        public THIS Link(Enemy character)
        {
            // 监听血量变化
            character.OnHpChangedEvent += val =>
            {
                this.Image.fillAmount = (float)val / character.Health;
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