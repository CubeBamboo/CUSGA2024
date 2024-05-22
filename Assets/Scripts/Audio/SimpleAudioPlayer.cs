using UnityEngine;

namespace Shuile.Audio
{
    public class SimpleAudioPlayer : AudioPlayerInUnity
    {
        public override AudioSource TargetSource => LevelAudioManager.Instance.MusicSource;

        public new void Reset()
        {
            TargetSource.Stop();
            TargetSource.clip = null;
            TargetSource.volume = 1f;
        }
    }
}
