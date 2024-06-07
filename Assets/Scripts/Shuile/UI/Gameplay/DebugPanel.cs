/* [WARNING]
 * only for debug purpose
 * remove this script in the final relese
 */

using Shuile.Framework;
using Shuile.Rhythm.Runtime;
using Shuile.Gameplay;
using CbUtils.Event;

using UnityEngine;
using TMPro;
using CbUtils.Preview.Event;
using Shuile.Core.Framework;
using Shuile.Core;
using Shuile.Model;

namespace Shuile
{
    // 写成屎了，不过调试用就算了
    public class DebugPanel : BasePanelWithMono, IEntity
    {
        private MusicRhythmManager _musicRhythmManager;

        [SerializeField] private TextMeshProUGUI hitOffsetText;
        [SerializeField] private TextMeshProUGUI playTimeText;
        [SerializeField] private TextMeshProUGUI dangerScoreText;
        [SerializeField] private TextMeshProUGUI dangerLevelText;
        [SerializeField] private TextMeshProUGUI enemyCountText;

        private PlayerModel playerModel;
        private LevelModel levelModel;

        private void Awake()
            => this.RegisterUI<DebugPanel>();
        private void OnDestroy()
            => this.UnRegisterUI<DebugPanel>();

        private void Start()
        {
            playerModel = this.GetModel<PlayerModel>();
            levelModel = this.GetModel<LevelModel>();
            _musicRhythmManager = MusicRhythmManager.Instance;

            gameObject.AddComponent<UpdateEventMono>().OnFixedUpdate += () => //TODO: not a good way
            {
                hitOffsetText.text = $"HitOffset: {playerModel.currentHitOffset:0.000}";
                playTimeText.text = $"PlayTime: {_musicRhythmManager.CurrentTime:0.000}";
                dangerScoreText.text = $"DangerScore: {levelModel.DangerScore:0.000}";
                dangerLevelText.text = $"DangerLevel: {levelModel.DangerLevel}";
                enemyCountText.text = $"EnemyCount: {LevelEntityManager.Instance.EnemyCount}";
            };
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
