using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    // TODO: maybe panel is not a good idea... esspecially Hide() and Show()
    public class HUDHpBarElement : BasePanelInUnity
    {
        private GameObject _panel;
        private Image _image;

        public Image Image => _image;

        public override void BeforeInit()
        {
            var assets = Resources.Load<GameObject>("UIDesign/HUDHpBarElement");
            _panel = assets.Instantiate();
            _image = _panel.GetComponent<Image>();

            PlayerController player = GameplayService.Interface.Get<PlayerController>();

            // TODO: [!] unregister when not use (like Hide() called)
            // TODO: [!] abstract to object which have hp and transform (not like here only support player)
            // 监听血量变化
            player.OnHpChangedEvent += val =>
            {
                Image.fillAmount = (float)val / player.MaxHP;
            };
            // 跟随玩家
            player.gameObject.AddComponent<UpdateEventMono>().OnFixedUpdate += () =>
            {
                _panel.transform.position = player.transform.position + new Vector3(0, 1.5f); //Vector3(0, 1.5f): player height
            };
        }

        //public void Link(Transform follow, float height)
        //{

        //}

        protected override GameObject Panel => _panel;

        protected override Canvas Canvas => UICtrl.Instance.WorldCanvas;

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
