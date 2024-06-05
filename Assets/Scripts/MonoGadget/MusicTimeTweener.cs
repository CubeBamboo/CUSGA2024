using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoBehaviour, IEntity
    {
        private MusicRhythmManager manager;
        private float lastManagerTime;
        private float tweenTime;

        public float TweenTime => tweenTime;

        protected void Awake()
        {
            manager = MusicRhythmManager.Instance;
            tweenTime = lastManagerTime = manager.CurrentTime;
        }

        private void Update()
        {
            if (!manager.IsPlaying || lastManagerTime != manager.CurrentTime)
                tweenTime = lastManagerTime = manager.CurrentTime;
            else
                tweenTime += Time.deltaTime;
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
