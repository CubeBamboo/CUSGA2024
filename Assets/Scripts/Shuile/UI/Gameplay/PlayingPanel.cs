using Shuile.Gameplay;
using Shuile.Gameplay.Character;
using Shuile.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Gameplay
{
    public class PlayingPanel : MonoBehaviour
    {
        [SerializeField] private Image hpFillImage;
        [SerializeField] private TextMeshProUGUI dangerLevelText;
        [SerializeField] private Player player;

        private float _playerMaxHp;

        private void Start()
        {
            var scope = LevelScope.Interface;
            var levelModel = scope.GetImplementation<LevelModel>();
            levelModel.OnDangerScoreChange.Register(old =>
                    dangerLevelText.text = levelModel.DangerLevel.ToString())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _playerMaxHp = player.Property.maxHealthPoint;

            UpdateHpUI(int.MinValue, player.CurrentHp.Value);
            player.CurrentHp.Register(UpdateHpUI)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateHpUI(int oldHp, int newHp)
        {
            hpFillImage.fillAmount = newHp / _playerMaxHp;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
