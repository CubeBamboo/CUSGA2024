/* [WARNING]
 * only for debug purpose
 * remove this script in the final release
 */

using CbUtils.Preview.Event;
using Shuile.Core.Framework;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Entity;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using TMPro;
using UnityEngine;

namespace Shuile.UI.Gameplay
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

        private PlayerModel _playerModel;
        private LevelModel _levelModel;
        private LevelEntityManager _levelEntityManager;

        private void Awake()
            => this.RegisterUI<DebugPanel>();
        private void OnDestroy()
            => this.UnRegisterUI<DebugPanel>();

        private void Start()
        {
            var lifeTimeScope = LevelScope.Interface;
            _levelEntityManager = lifeTimeScope.GetImplementation<LevelEntityManager>();
            _playerModel = this.GetModel<PlayerModel>();
            _levelModel = this.GetModel<LevelModel>();
            _musicRhythmManager = MusicRhythmManager.Instance;

            gameObject.AddComponent<UpdateEventMono>().OnFixedUpdate += () => //TODO: not a good way
            {
                hitOffsetText.text = $"HitOffset: {_playerModel.currentHitOffset:0.000}";
                playTimeText.text = $"PlayTime: {_musicRhythmManager.CurrentTime:0.000}";
                dangerScoreText.text = $"DangerScore: {_levelModel.DangerScore:0.000}";
                dangerLevelText.text = $"DangerLevel: {_levelModel.DangerLevel}";
                enemyCountText.text = $"EnemyCount: {_levelEntityManager.EnemyCount}";
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
