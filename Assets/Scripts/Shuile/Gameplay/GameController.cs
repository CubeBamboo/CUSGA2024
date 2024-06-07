using Shuile.Audio;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class GameController : MonoBehaviour
    {
        private IAudioPlayer audioPlayer;  // = new NativeAudioPlayer();
        private IAudioPlayer bonusAudioPlayer;  // = new NativeAudioPlayer();

        /// <summary>
        /// BGM播放器
        /// </summary>
        public IAudioPlayer AudioPlayer => audioPlayer;
        /// <summary>
        /// 奖励音轨BGM播放器
        /// </summary>
        public IAudioPlayer BonusAudioPlayer => bonusAudioPlayer;
        /// <summary>
        /// 局内时间（音乐时间+延迟时间）
        /// </summary>
        public float GameTime { get; set; }

        private void Awake()
        {
            audioPlayer.LoadClip(null);  // Todo: Load audio
            bonusAudioPlayer.LoadClip(null);  // Todo: Load bonus audio
        }
    }
}
