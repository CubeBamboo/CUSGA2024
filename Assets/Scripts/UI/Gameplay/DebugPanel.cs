/* [WARNING]
 * only for debug purpose
 * remove this script in the final relese
 */

using Shuile.Framework;
using CbUtils;

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Shuile.Rhythm;
using Shuile.Gameplay;

namespace Shuile
{
    // 写成屎了，不过调试用就算了
    public class DebugPanel : BasePanelWithMono
    {
        [SerializeField] private TextMeshProUGUI hitOffsetText;
        [SerializeField] private TextMeshProUGUI playTimeText;
        [SerializeField] private Toggle enemyClampToggle;

        private void Awake()
            => this.RegisterUI<DebugPanel>();
        private void OnDestroy()
            => this.UnRegisterUI<DebugPanel>();

        private void Start()
        {
            var playerModel = GameplayService.Interface.Get<PlayerModel>();
            gameObject.AddComponent<UpdateEventMono>().OnFixedUpdate += () => //TODO: not a good way
            {
                hitOffsetText.text = $"HitOffset: " + playerModel.currentHitOffset.ToString("0.000");
                playTimeText.text = $"PlayTime: " + MusicRhythmManager.Instance.CurrentTime.ToString("0.000");
            };
            enemyClampToggle.isOn = DebugProperty.Instance.GetInt("EnemyClamp") == 1 ? true : false;
            enemyClampToggle.onValueChanged.AddListener((isOn) =>
            {
                DebugProperty.Instance.SetInt("EnemyClamp", isOn ? 1 : 0);
            });
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
