using CbUtils;
using CbUtils.Extension;
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
    
    public class LevelAudioManager : CSharpLazySingletons<LevelAudioManager>
    {
        private MonoLevelAudioManager gameObject;

        public AudioSource MusicSource => gameObject.MusicSource;
        public AudioSource SfxSource => gameObject.SfxSource;

        public LevelAudioManager()
        {
            gameObject = Resources.Load<GameObject>("GlobalGameObject/LevelAudioManager")
                .Instantiate()
                .SetDontDestroyOnLoad()
                .GetComponent<MonoLevelAudioManager>();
        }
        ~LevelAudioManager()
        {
            gameObject.Destroy();
        }
    }
}
