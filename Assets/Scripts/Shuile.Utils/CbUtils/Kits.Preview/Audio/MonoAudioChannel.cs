using Shuile.Utils;
using UnityEngine;

namespace Shuile
{
    public class MonoAudioChannel : MonoBehaviour
    {
        private AudioSource _audioSource;
        private ResourceLoader _resourceLoader;

        private void Awake()
        {
            _audioSource = gameObject.GetComponent<AudioSource>().ThrowIfNull();
            _resourceLoader = new ResourceLoader();
        }

        public void Play(string addressableKey)
        {
            var clip = _resourceLoader.Load<AudioClip>(addressableKey);
            Play(clip);
        }

        public void Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}
