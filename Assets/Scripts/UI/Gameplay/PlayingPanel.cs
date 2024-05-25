using UnityEngine;
using TMPro;

using Shuile.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay.Event;
using UnityEngine.UI;
using Shuile.Model;
using Shuile.Core.Framework;


namespace Shuile.UI
{
    public class PlayingPanel : BasePanelWithMono, IEntity
    {
        [SerializeField] private Image hpFillImage;
        [SerializeField] private TextMeshProUGUI dangerLevelText;

        private float playerMaxHp;

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
            playerMaxHp = player.Property.maxHealthPoint;

            UpdateHpUI(int.MinValue, player.CurrentHp.Value);
            player.CurrentHp.Register(UpdateHpUI)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            var levelModel = this.GetModel<LevelModel>();
            levelModel.OnDangerScoreChange.Register(old =>
                dangerLevelText.text = levelModel.DangerLevel.ToString())
               .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateHpUI(int oldHp, int newHp)
        {
            hpFillImage.fillAmount = newHp / playerMaxHp;
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public void OnSelfEnable()
        {
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
