//[WIP]

using Cysharp.Threading.Tasks;
using Shuile.Audio;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile
{
    public class AudioOffsetDebugger : MonoBehaviour
    {
        public bool useDefaultPlayerManager = true;

        //private AudioSource audioSource;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float spriteLength;
        private AudioSource audioSource;

        async void Start()
        {
            await UniTask.Delay(200);
            // init audio source
            if (useDefaultPlayerManager)
            {
                var audioPlayer = MusicRhythmManager.Instance.AudioPlayer as SimpleAudioPlayer;
                audioSource = audioPlayer.TargetSource;
            }
            else
            {
                CbLogger.LogWarning($"Custom player manager still not support, make sure you have a {nameof(MusicRhythmManager)} in game sccene", "AudioOffsetDebugger");
            }

            // init waveform sprite
            var texture2D = AudioClipUtility.BakeAudioWaveform(audioSource.clip, 240, 8000, 400);

            spriteRenderer.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.0f, 0.5f));
            spriteRenderer.transform.localScale = new Vector3(5, 1, 1);

            spriteLength = spriteRenderer.sprite.bounds.size.x * spriteRenderer.transform.lossyScale.x;
        }

        private void Update()
        {
            if (!audioSource) return;
            //Debug.Log(audioSource.isPlaying);
            //Debug.Log(audioSource.time + ", " + audioSource.clip.length);
            var progress = audioSource.time / audioSource.clip.length;
            var spritePosition = -spriteLength * progress;
            spriteRenderer.transform.localPosition = new Vector3(spritePosition, 0, 0);

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Retry();
            }
        }

        private void Retry()
        {
            MusicRhythmManager.Instance.RestartPlay();
        }
    }
}
