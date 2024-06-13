using CbUtils.Unity;
using Shuile.Chart;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.Persistent;
using Shuile.ResourcesManagement.Loader;
using System;
using UnityEngine;

namespace Shuile.Rhythm
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoBehaviour
    {
        private LevelModel _levelModel;

        private PreciseMusicPlayer _preciseMusicPlayer;
        private LevelConfigSO _levelConfig;

        private ChartData _currentChart;

        public bool playOnAwake = true;
        public float playTimeScale = 1f;
        public float volume = 0.4f;

        public bool IsPlaying { get; private set; } = false;
        public float CurrentTime => _levelModel.CurrentMusicTime;
        public float MusicLength => _currentChart.musicLength;
        public bool IsMusicEnd => CurrentTime >= MusicLength;

        private void Awake()
        {
            var scope = LevelScope.Interface;
            _levelModel = scope.GetImplementation<LevelModel>();
            _preciseMusicPlayer = scope.GetImplementation<PreciseMusicPlayer>();

            var resourcesLoader = LevelResourcesLoader.Instance;
            _levelConfig = resourcesLoader.SyncContext.levelConfig;
        }

        private void Start()
        {
            RefreshData();
            if (playOnAwake)
                StartPlay();
        }

        public void RefreshData()
        {
            _currentChart = LevelRoot.LevelContext.ChartData;
            
            playOnAwake = _levelConfig.playOnAwake;
            playTimeScale = _levelConfig.playTimeScale;
            volume = _levelConfig.volume;

            _preciseMusicPlayer.ReloadData();
        }

        public void StartPlay()
        {
            float offsetInSeconds = (_currentChart.time[0].offset + MainGame.Interface.Get<Config>().GlobalDelay) * 0.001f;
            Time.timeScale = playTimeScale;

            _preciseMusicPlayer.StartPlay(offsetInSeconds);
        }

        public void StopPlay() => _preciseMusicPlayer.Stop();

        public void RestartPlay()
        {
            _preciseMusicPlayer.ReloadData();
            StartPlay();
        }

        public void FadeOutAndStop(float duration = 0.8f) => _preciseMusicPlayer.FadeOutAndStop(duration);

        public void SetCurrentTime(float time) => _preciseMusicPlayer.SetCurrentTime(time);
    }
}
