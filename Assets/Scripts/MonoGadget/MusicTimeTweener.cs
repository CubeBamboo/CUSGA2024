using Shuile.Core.Framework;
using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoEntity
    {
        private MusicRhythmManager manager;
        private float lastManagerTime;
        private float tweenTime;

        public float TweenTime => tweenTime;

        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;

        protected override void AwakeOverride()
        {
            manager = this.GetSystem<MusicRhythmManager>();
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
