/* [WARNING]
 * only for debug purpose
 * remove this script in the final release
 */

using Shuile.Gameplay;
using Shuile.Gameplay.Entity;
using Shuile.Model;
using Shuile.Rhythm;
using TMPro;
using UnityEngine;

namespace Shuile.UI.Gameplay
{
    // 写成屎了，不过调试用就算了
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hitOffsetText;
        [SerializeField] private TextMeshProUGUI playTimeText;
        [SerializeField] private TextMeshProUGUI dangerScoreText;
        [SerializeField] private TextMeshProUGUI dangerLevelText;
        [SerializeField] private TextMeshProUGUI enemyCountText;
        private LevelEntityManager _levelEntityManager;
        private LevelModel _levelModel;
        private MusicRhythmManager _musicRhythmManager;

        private PlayerModel _playerModel;

        private void Start()
        {
            var scope = LevelScope.Interface;
            _levelEntityManager = scope.GetImplementation<LevelEntityManager>();
            _playerModel = scope.GetImplementation<PlayerModel>();
            _levelModel = scope.GetImplementation<LevelModel>();

            _musicRhythmManager = scope.GetImplementation<MusicRhythmManager>();
        }

        private void FixedUpdate()
        {
            hitOffsetText.text = $"HitOffset: {_playerModel.currentHitOffset:0.000}";
            playTimeText.text = $"PlayTime: {_musicRhythmManager.CurrentTime:0.000}";
            dangerScoreText.text = $"DangerScore: {_levelModel.DangerScore:0.000}";
            dangerLevelText.text = $"DangerLevel: {_levelModel.DangerLevel}";
            enemyCountText.text = $"EnemyCount: {_levelEntityManager.EnemyCount}";
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
