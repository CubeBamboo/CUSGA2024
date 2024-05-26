using CbUtils;
using UnityEngine;

namespace Shuile.Audio
{
    public class LevelAudioManager : MonoNotAutoSpawnSingletons<LevelAudioManager>
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        public AudioSource MusicSource => musicSource;
        public AudioSource SfxSource => sfxSource;
    }
}
