using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Audio;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using System;
using System.Threading;
using UnityEngine;

namespace Shuile.Rhythm
{
    /// <summary>
    ///     it provides an async player for lower delay, a timer start from music's specific position
    /// </summary>
    public class PreciseMusicPlayer : IFixedTickable, IDestroyable
    {
        private readonly LevelAudioManager _levelAudioManager;
        private readonly LevelConfigSO _levelConfig;
        private readonly LevelModel _levelModel;

        private CancellationTokenSource _asyncPlayTokenSource;

        public PreciseMusicPlayer(RuntimeContext context)
        {
            _levelAudioManager = context.GetImplementation<LevelAudioManager>();
            _levelModel = context.GetImplementation<LevelModel>();
            context.Resolve(out UnityEntryPointScheduler scheduler);
            scheduler.AddOnce(Restore);
            scheduler.AddFixedUpdate(FixedTick);
            scheduler.AddCallOnDestroy(OnDestroy);

            var resourcesLoader = LevelResourcesLoader.Instance;
            _levelConfig = resourcesLoader.SyncContext.levelConfig;

            AudioPlayer = new UnityAudioPlayer(_levelAudioManager.MusicSource);
        }

        public UnityAudioPlayer AudioPlayer { get; private set; }

        /// <summary> timer for music playing by play offset </summary>
        public float CurrentTime { get; private set; }

        public bool IsPlaying { get; set; }
        public float PlayTimeScale { get; set; } = 1f;

        public float Volume
        {
            get => AudioPlayer.Volume;
            set => AudioPlayer.Volume = value;
        }

        public void OnDestroy()
        {
            AudioPlayer.Stop();
        }

        public void FixedTick()
        {
            if (!IsPlaying)
            {
                return;
            }

            CurrentTime += Time.fixedDeltaTime;
            _levelModel.CurrentMusicTime = CurrentTime;
        }

        public void LoadClip(AudioClip clip)
        {
            AudioPlayer.LoadClip(clip);
        }

        public async UniTask Play(float offsetInSeconds, float startDelay = 0.5f)
        {
            _asyncPlayTokenSource = new CancellationTokenSource();

            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: _asyncPlayTokenSource.Token);
            AudioPlayer.Pitch = PlayTimeScale;

            var playAt = AudioSettings.dspTime + 1f;
            var audioDelta = await AudioPlayer.WaitPlayScheduled(playAt, _asyncPlayTokenSource.Token);

            CurrentTime = -offsetInSeconds + (float)audioDelta;
            IsPlaying = true; // -> start timing
        }

        public void PlayImmediately(float offset)
        {
            AudioPlayer.Pitch = PlayTimeScale;
            AudioPlayer.Play();
            CurrentTime = -offset;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            AudioPlayer.Stop();
            _asyncPlayTokenSource?.Cancel();
        }

        /// <summary>
        ///     default need to call in FixedUpdate
        /// </summary>
        public void Restore()
        {
            CurrentTime = 0;
            IsPlaying = false;
            AudioPlayer.Reset();
        }

        public void SetCurrentTime(float time)
        {
            time = Mathf.Clamp(time, 0f, AudioPlayer.TargetSource.clip.length);

            CurrentTime = time;
            AudioPlayer.TargetSource.time = time;
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
    }
}
