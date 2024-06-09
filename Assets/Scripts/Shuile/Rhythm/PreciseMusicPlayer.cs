using CbUtils.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Audio;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using System.Threading;
using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// it provides an async player for lower delay, a timer start from music's specific position
    /// </summary>
    public class PreciseMusicPlayer : IEntity, IInitializeable, IFixedTickable, IDestroyable
    {
        private LevelConfigSO _levelConfig;

        private LevelAudioManager _levelAudioManager;
        private LevelModel _levelModel;
        private UnityAudioPlayer _audioPlayer;
        public UnityAudioPlayer AudioPlayer => _audioPlayer;
        
        public void Initialize()
        {
            var sceneLocator = LevelScope.Interface;
            var resourcesLoader = LevelResourcesLoader.Instance;
            
            _levelConfig = resourcesLoader.SyncContext.levelConfig;
            _levelAudioManager = sceneLocator.Get<LevelAudioManager>();
            _levelModel = this.GetModel<LevelModel>();

            _audioPlayer = new UnityAudioPlayer(_levelAudioManager.MusicSource);
            Restore();
        }
        public void OnDestroy()
        {
            _audioPlayer.Stop();
        }
        public void FixedTick()
        {
            if (!IsPlaying) return;
            CurrentTime += Time.fixedDeltaTime;
            _levelModel.currentMusicTime = CurrentTime;
        }

        /// <summary> timer for music playing by play offset </summary>
        public float CurrentTime { get; private set; } = 0f;
        public bool IsPlaying { get; set; } = false;
        public float PlayTimeScale { get; set; } = 1f;
        public float Volume
        {
            get => _audioPlayer.Volume;
            set => _audioPlayer.Volume = value;
        }

        private CancellationTokenSource _asyncPlayTokenSource;

        public void LoadClip(AudioClip clip)
        {
            _audioPlayer.LoadClip(clip);
        }

        public async UniTask Play(float offsetInSeconds, float startDelay = 0.5f)
        {
            _asyncPlayTokenSource = new();
            
            await UniTask.Delay(System.TimeSpan.FromSeconds(startDelay), cancellationToken: _asyncPlayTokenSource.Token);
            _audioPlayer.Pitch = PlayTimeScale;

            var playAt = AudioSettings.dspTime + 1f;
            var audioDelta = await _audioPlayer.WaitPlayScheduled(playAt, _asyncPlayTokenSource.Token);

            CurrentTime = -offsetInSeconds + (float)audioDelta;
            IsPlaying = true; // -> start timing
        }

        public void PlayImmediately(float offset)
        {
            _audioPlayer.Pitch = PlayTimeScale;
            _audioPlayer.Play();
            CurrentTime = -offset;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            _audioPlayer.Stop();
            _asyncPlayTokenSource?.Cancel();
        }

        /// <summary>
        /// default need to call in FixedUpdate
        /// </summary>
        public void Restore()
        {
            CurrentTime = 0;
            IsPlaying = false;
            _audioPlayer.Reset();
        }

        public void SetCurrentTime(float time)
        {
            time = Mathf.Clamp(time, 0f, _audioPlayer.TargetSource.clip.length);

            CurrentTime = time;
            _audioPlayer.TargetSource.time = time;
        }

        public void ReloadData()
        {
            Restore();
            LoadClip(LevelRoot.LevelContext.ChartData.audioClip);
        }

        public void StartPlay(float offset)
        {
            Volume = _levelConfig.volume;
            Play(offset).Forget();
        }

        public void FadeOutAndStop(float duration)
        {
            AudioPlayer.TargetSource.DOFade(0, duration).OnComplete(() => AudioPlayer.Stop());
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
