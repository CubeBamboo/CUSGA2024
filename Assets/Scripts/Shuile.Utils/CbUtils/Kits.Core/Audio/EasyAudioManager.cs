using CbUtils.Unity;
using UnityEngine;
using UnityEngine.Audio;

namespace CbUtils
{
    public class EasyAudioManager : MonoSingletons<EasyAudioManager>
    {
        private AudioSource bgmSource, sfxSource, voiceSource;

        public AudioSource OneShotSource { get; private set; }

        public AudioSource OtherSource { get; private set; }

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
            OtherSource = gameObject.AddComponent<AudioSource>();
            OtherSource.playOnAwake = false;
            OneShotSource = gameObject.AddComponent<AudioSource>();
            OneShotSource.playOnAwake = false;
        }

        public void SetAudioMixer(AudioMixerGroup mixer)
        {
            OtherSource.outputAudioMixerGroup = mixer;
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
            OtherSource.clip = clip;
            OtherSource.Play();
        }

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            OneShotSource.PlayOneShot(clip, volumeScale);
        }

        #endregion
    }
}
