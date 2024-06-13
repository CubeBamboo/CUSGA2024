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
using System;
using TMPro;
using UnityEngine;

namespace Shuile.UI.Gameplay
{
    // 写成屎了，不过调试用就算了
    public class DebugPanel : MonoBehaviour, IEntity
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
            //TODO: not a good way
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

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
