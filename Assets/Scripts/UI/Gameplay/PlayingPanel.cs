using UnityEngine;
using TMPro;

using Shuile.Gameplay;
using Shuile.Framework;


namespace Shuile.UI
{
    public class PlayingPanel : BasePanelWithMono
    {
        [SerializeField] private TextMeshProUGUI hpTextUGUI;

        public TextMeshProUGUI HpTextUGUI => hpTextUGUI;
        private void Awake()
            => this.RegisterUI<PlayingPanel>();
        private void OnDestroy()
            => this.UnRegisterUI<PlayingPanel>();

        private void Start()
        {
            GameplayService.Interface.Get<Player>().CurrentHp.Register((oldV, newV) =>
            {
                hpTextUGUI.text = "Player HP: " + (newV > 0 ? newV : 0).ToString();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
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
