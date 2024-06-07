using UnityEngine;
using Shuile.Framework;
using Shuile.Persistent;
using Shuile.ResourcesManagement.Loader;
using Shuile.Core.Framework;
using Shuile.Model;
using CbUtils.Unity;
using Shuile.Core;
using Shuile.Root;

namespace Shuile.Rhythm.Runtime
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>, IEntity
    {
        private LevelModel _levelModel;

        private PreciseMusicPlayer _preciseMusicPlayer;
        private LevelConfigSO levelConfig;

        private ChartData currentChart;
        private bool isPlaying = false;

        public bool playOnAwake = true;
        public float playTimeScale = 1f;
        public float volume = 0.4f;

        public bool IsPlaying => isPlaying;
        public float CurrentTime => _levelModel.currentMusicTime;
        public float MusicLength => currentChart.musicLength;
        public bool IsMusicEnd => CurrentTime >= MusicLength;

        protected override void OnAwake()
        {
            _levelModel = this.GetModel<LevelModel>();
            _preciseMusicPlayer = PreciseMusicPlayer.Instance;

            levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
        }

        private void Start()
        {
            RefreshData();
            if (playOnAwake)
                StartPlay();
        }

        public void RefreshData()
        {
            currentChart = LevelRoot.LevelContext.ChartData;
            
            playOnAwake = levelConfig.playOnAwake;
            playTimeScale = levelConfig.playTimeScale;
            volume = levelConfig.volume;

            _preciseMusicPlayer.ReloadData();
        }

        public void StartPlay()
        {
            float offsetInSeconds = (currentChart.time[0].offset + MainGame.Interface.Get<Config>().GlobalDelay) * 0.001f;
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

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
