using CbUtils.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Audio;
using Shuile.Core;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm.Runtime;
using System.Threading;
using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// it provide a async player for lower delay, a timer start from music's specific position
    /// </summary>
    // TODO: it designed for a utils class in the first time, but now it's implemented to be a entity, and it needs to refactor
    public class PreciseMusicPlayer : MonoEntity
    {
        public static PreciseMusicPlayer Instance => MonoSingletonProperty<PreciseMusicPlayer>.Instance;

        private LevelConfigSO levelConfig;

        private MusicRhythmManager _musicRhythmManager;
        private LevelModel levelModel;
        private AudioPlayerInUnity audioPlayer;
        public AudioPlayerInUnity AudioPlayer => audioPlayer;
        protected override void AwakeOverride()
        {
            levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;

            _musicRhythmManager = this.GetSystem<MusicRhythmManager>();
            levelModel = this.GetModel<LevelModel>();

            audioPlayer = new SimpleAudioPlayer();
            Restore();
            InitializeEvent();
        }

        public void FixedUpdate()
        {
            if (!IsPlaying) return;
            CurrentTime += Time.fixedDeltaTime;
            levelModel.currentMusicTime = CurrentTime;
        }

        /// <summary> timer for music playing by play offset </summary>
        public float CurrentTime { get; private set; } = 0f;
        public bool IsPlaying { get; set; } = false;
        public float PlayTimeScale { get; set; } = 1f;
        public float Volume
        {
            get => audioPlayer.Volume;
            set => audioPlayer.Volume = value;
        }

        private CancellationTokenSource asyncPlayTokenSource;

        public void LoadClip(AudioClip clip)
        {
            audioPlayer.LoadClip(clip);
        }

        public async UniTask Play(float offsetInSeconds, float startDelay = 0.5f)
        {
            asyncPlayTokenSource = new();
            
            await UniTask.Delay(System.TimeSpan.FromSeconds(startDelay), cancellationToken: asyncPlayTokenSource.Token);
            audioPlayer.Pitch = PlayTimeScale;

            var playAt = AudioSettings.dspTime + 1f;
            var audioDelta = await audioPlayer.WaitPlayScheduled(playAt, asyncPlayTokenSource.Token);

            CurrentTime = -offsetInSeconds + (float)audioDelta;
            IsPlaying = true; // -> start timing
        }

        public void PlayImmediatly(float offset)
        {
            audioPlayer.Pitch = PlayTimeScale;
            audioPlayer.Play();
            CurrentTime = -offset;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            audioPlayer.Stop();
            asyncPlayTokenSource?.Cancel();
        }

        /// <summary>
        /// default need to call in FixedUpdate
        /// </summary>

        public void Restore()
        {
            CurrentTime = 0;
            IsPlaying = false;
            audioPlayer.Reset();
        }

        public void SetCurrentTime(float time)
        {
            time = Mathf.Clamp(time, 0f, audioPlayer.TargetSource.clip.length);

            CurrentTime = time;
            audioPlayer.TargetSource.time = time;
        }

        private void InitializeEvent()
        {
            _musicRhythmManager.OnReloadData.Register(() =>
            {
                Restore();
                LoadClip(LevelDataBinder.Instance.ChartData.audioClip);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            _musicRhythmManager.OnStartPlay.Register(offset =>
            {
                Volume = levelConfig.volume;
                Play(offset).Forget();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            _musicRhythmManager.OnStopPlay.Register(Stop)
              .UnRegisterWhenGameObjectDestroyed(gameObject);
            _musicRhythmManager.OnFadeOutAndStop.Register(duration =>
            {
                AudioPlayer.TargetSource.DOFade(0, duration).OnComplete(() => AudioPlayer.Stop());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            _musicRhythmManager.OnSetCurrentTime.Register(SetCurrentTime)
              .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public override ModuleContainer GetModule() => GameApplication.Level;
    }
}
