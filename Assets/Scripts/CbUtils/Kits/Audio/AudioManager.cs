using UnityEngine;

namespace CbUtils
{
    public class AudioManager : CbUtils.MonoSingletons<AudioManager>
    {
        private AudioSource bgmSource, sfxSource, voiceSource, otherSource;
        private AudioSource oneShotSource;

        public AudioSource OneShotSource;
        public AudioSource OtherSource => otherSource;

        protected override void Awake()
        {
            base.Awake();
            InitComponent();
            SetDontDestroyOnLoad();
        }

        private void InitComponent()
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.playOnAwake = false;
            otherSource = gameObject.AddComponent<AudioSource>();
            otherSource.playOnAwake = false;
            oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
        }

        #region PlayAudio


        public void PlayBgm(AudioClip clip)
        {
            bgmSource.Stop();
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            sfxSource.clip = clip;
            sfxSource.Play();
        }

        public void PlayVoice(AudioClip clip)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
        }

        public void PlayOther(AudioClip clip)
        {
            otherSource.clip = clip;
            otherSource.Play();
        }

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            oneShotSource.PlayOneShot(clip, volumeScale);
        }

        #endregion
    }
}
