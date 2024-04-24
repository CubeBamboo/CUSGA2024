using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    [RequireComponent(typeof(MusicRhythmManager))]
    public class MusicTimeTweener : MonoBehaviour
    {
        private MusicRhythmManager manager;
        private float lastManagerTime;
        private float tweenTime;

        public float TweenTime => tweenTime;

        private void Awake()
        {
            manager = GetComponent<MusicRhythmManager>();
            tweenTime = lastManagerTime = manager.CurrentTime;
        }

        private void Update()
        {
            if (!manager.IsPlaying || lastManagerTime != MusicRhythmManager.Instance.CurrentTime)
                tweenTime = lastManagerTime = MusicRhythmManager.Instance.CurrentTime;
            else
                tweenTime += Time.deltaTime;
        }
    }
}