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
        /// 音乐时间（更新不一定及时）
        /// </summary>
        float Time { get; set; }
        /// <summary>
        /// 音量
        /// </summary>
        float Volume { get; set; }
    }
}
