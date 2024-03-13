using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils
{
    public class AudioManager : CbUtils.MonoSingletons<AudioManager>
    {
        private AudioSource bgmSource, sfxSource, voiceSource, otherSource;

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
            sfxSource = gameObject.AddComponent<AudioSource>();
            voiceSource = gameObject.AddComponent<AudioSource>();
            otherSource = gameObject.AddComponent<AudioSource>();
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

        #endregion
    }
}