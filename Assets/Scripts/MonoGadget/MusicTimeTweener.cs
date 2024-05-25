using Shuile.Core;
using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoBehaviour
    {
        private IMusicRhythmManager manager;
        private float lastManagerTime;
        private float tweenTime;

        public float TweenTime => tweenTime;

        private void Awake()
        {
            manager = GameApplication.ServiceLocator.GetService<IMusicRhythmManager>();
            tweenTime = lastManagerTime = manager.CurrentTime;
        }

        private void Update()
        {
            if (!manager.IsPlaying || lastManagerTime != manager.CurrentTime)
                tweenTime = lastManagerTime = manager.CurrentTime;
            else
                tweenTime += Time.deltaTime;
        }
    }
}
