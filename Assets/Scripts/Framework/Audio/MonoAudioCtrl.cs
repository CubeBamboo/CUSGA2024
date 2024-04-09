using CbUtils;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// recommend to use in fast-development stage
    /// </summary>
    public class MonoAudioCtrl : MonoSingletons<MonoAudioCtrl>
    {
        [Serializable]
        private class ClipData
        {
            public string id;
            public AudioClip clip;
        }

        public enum AudioChannel
        {
            Bgm, SFX, Voice
        }

        [SerializeField] private List<ClipData> clipList;
        private Dictionary<string, AudioClip> clipDictionary = new();

        protected override void OnAwake()
        {
            foreach (ClipData data in clipList)
            {
                if (!clipDictionary.TryAdd(data.id, data.clip))
                    Debug.LogWarning($"{data.id} has already add to AudioCtrl");
            }
        }

        public void Play(string id, AudioChannel channel)
        {
            if (clipDictionary.TryGetValue(id, out var clip))
            {
                switch (channel)
                {
                    case AudioChannel.Bgm:
                        AudioManager.Instance.PlayBgm(clip);
                        break;
                    case AudioChannel.SFX:
                        AudioManager.Instance.PlaySFX(clip);
                        break;
                    case AudioChannel.Voice:
                        AudioManager.Instance.PlayVoice(clip);
                        break;
                }
                return;
            }

            Debug.LogError($"audio with id \"{id}\" was not found");
        }
        public void PlayOneShot(string id, float volumeScale = 1)
        {
            if (clipDictionary.TryGetValue(id, out var clip))
            {
                AudioManager.Instance.PlayOneShot(clip, volumeScale);
                return;
            }
            Debug.LogError($"audio with id \"{id}\" was not found");
        }

        public AudioClip Get(string id)
            => clipDictionary[id];

        public void PlaySFX(string id) => Play(id, AudioChannel.SFX);
    }

}
