using UnityEngine;
using TMPro;

using Shuile.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay.Event;


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

        private void OnEnable()
            => LevelLoadEndEvent.Register(OnStart);
        private void OnDisable()
            => LevelLoadEndEvent.UnRegister(OnStart);

        private void OnStart(string sceneName)
        {
            var player = GameplayService.Interface.Get<Player>();
            UpdateHpText(int.MinValue, player.CurrentHp.Value);
            player.CurrentHp.Register(UpdateHpText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateHpText(int oldHp, int newHp)
        {
            hpTextUGUI.text = "Player HP: " + (newHp > 0 ? newHp : 0).ToString();
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
