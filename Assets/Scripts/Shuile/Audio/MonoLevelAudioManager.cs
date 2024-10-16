using UnityEngine;

namespace Shuile.Audio
{
    public class MonoLevelAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        public AudioSource MusicSource => musicSource;
        public AudioSource SfxSource => sfxSource;
    }
}
