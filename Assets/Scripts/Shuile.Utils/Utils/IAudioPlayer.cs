using UnityEngine;

namespace Shuile.Audio
{
    public interface IAudioPlayer
    {
        /// <summary>
        /// 加载音频
        /// </summary>
        /// <param name="clip">音频</param>
        void LoadClip(AudioClip clip);
        /// <summary>
        /// 播放/恢复
        /// </summary>
        void Play();
        /// <summary>
        /// 在特定时间播放
        /// </summary>
        void PlayScheduled(double time);
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        /// <summary>
        /// 重置状态
        /// </summary>
        void Reset();
        /// <summary>
        /// 音乐时间（更新不一定及时）
        /// </summary>
        float Time { get; set; }
        /// <summary>
        /// 音量
        /// </summary>
        float Volume { get; set; }
        float Pitch { get; set; }
        double AudioSystemTime { get; }
    }

    public abstract class AudioPlayerInUnity : IAudioPlayer
    {
        public abstract AudioSource TargetSource { get;}
        public float Time
        {
            get => TargetSource.time;
            set => TargetSource.time = value;
        }
        public float Volume
        {
            get => TargetSource.volume;
            set => TargetSource.volume = value;
        }
        public float Pitch
        {
            get => TargetSource.pitch;
            set => TargetSource.pitch = value;
        }
        public double AudioSystemTime => AudioSettings.dspTime;

        public void LoadClip(AudioClip clip)
        {
            TargetSource.clip = clip;
        }
        public void Play()
        {
            TargetSource.Play();
        }
        public void PlayScheduled(double time)
        {
            TargetSource.PlayScheduled(time);
        }
        public void Pause()
        {
            TargetSource.Pause();
        }
        public void Stop()
        {
            TargetSource.Stop();
        }
        public void Reset()
        {
            TargetSource.Stop();
            TargetSource.time = 0;
        }
    }
}
