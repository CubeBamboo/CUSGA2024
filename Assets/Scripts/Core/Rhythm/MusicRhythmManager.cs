using UnityEngine;
using Shuile.Framework;
using Shuile.Persistent;
using Shuile.ResourcesManagement.Loader;
using Shuile.Gameplay.Event;
using Shuile.Core.Framework;
using Shuile.Model;
using CbUtils;

namespace Shuile.Rhythm.Runtime
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : ISystem
    {
        private LevelModel levelModel;
        private LevelConfigSO levelConfig;

        public EasyEvent OnReloadData = new(), OnStopPlay = new(), OnRestartPlay = new();
        /// <summary> param: offset in secods </summary>
        public EasyEvent<float> OnStartPlay = new();
        public EasyEvent<float> OnFadeOutAndStop = new (),  OnSetCurrentTime = new();

        private ChartData currentChart;
        private bool isPlaying = false;

        //private PreciseMusicPlayer preciseMusicPlayer;

        public bool playOnAwake = true;
        public float playTimeScale = 1f;
        public float volume = 0.4f;

        public bool IsPlaying => isPlaying;
        public float CurrentTime => levelModel.currentMusicTime;
        public float MusicLength => currentChart.musicLength;
        public bool IsMusicEnd => CurrentTime >= MusicLength;

        public MusicRhythmManager()
        {
            levelModel = this.GetModel<LevelModel>();
            levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;

            LevelStartEvent.Register(name =>
            {
                RefreshData();
                if (playOnAwake)
                    StartPlay();
            });
        }

        public void RefreshData()
        {
            currentChart = LevelDataBinder.Instance.ChartData;
            
            playOnAwake = levelConfig.playOnAwake;
            playTimeScale = levelConfig.playTimeScale;
            volume = levelConfig.volume;

            OnReloadData.Invoke();
        }

        public void StartPlay()
        {
            float offsetInSeconds = (currentChart.time[0].offset + MainGame.Interface.Get<Config>().GlobalDelay) * 0.001f;
            Time.timeScale = playTimeScale;

            OnStartPlay.Invoke(offsetInSeconds);
        }

        public void StopPlay() => OnStopPlay?.Invoke();

        public void RestartPlay()
        {
            OnReloadData.Invoke();
            StartPlay();
        }

        public void FadeOutAndStop(float duration = 0.8f) => OnFadeOutAndStop.Invoke(duration);

        public void SetCurrentTime(float time) => OnSetCurrentTime.Invoke(time);

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
